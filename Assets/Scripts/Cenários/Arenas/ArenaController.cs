using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public enum ArenaTypes
{
    DistanceCamera,
    StayCamera,
    TargetEnemyCamera
}

public class ArenaController : MonoBehaviour
{
    public int ArenaID { get; private set; }

    [SerializeField] private List<GameObject> closesDoors = new List<GameObject>();
    [SerializeField] private List<Animator> animatorsDoors = new List<Animator>();
    [SerializeField] private bool startedArena;
    public bool AuthorizeSpawn { get; private set; }

    [Header("Camera Intro")]
    [SerializeField] private Transform cameraFocusPoint;
    [SerializeField] private Transform stayCameraCenterPoint;
    [SerializeField] private float cameraPanTime = 0.5f;
    [SerializeField] private float cameraStayDuration = 1f;
    [SerializeField] private bool lockPlayerDuringCamera = true;
    [SerializeField] private float distanceCameraLensIncrease = 2f;
    [SerializeField] private float lensTransitionTime = 0.35f;

    private float timeToStartArena = 2f;
    private float timer;

    private CinemachineVirtualCamera cinemachineVirtualCamera;
    private CinemachineFramingTransposer framingTransposer;
    private PlayerMoviment playerMoviment;
    private Coroutine arenaIntroCameraCoroutine;
    private Coroutine lensTransitionCoroutine;
    private Transform defaultFollowTarget;
    private Transform targetEnemyCameraCenterPoint;
    private float defaultOrthographicSize;
    private bool stayCameraLocked;
    private bool targetEnemyCameraLocked;
    private bool enforceLensSize;
    private float enforcedLensSize;
    private Collider2D arenaBoundsCollider;
    private readonly List<Damage> arenaEnemies = new List<Damage>();

    public ArenaTypes arenaType;

    void Awake()
    {
        //Os dois primeiros filhos são as portas.
        closesDoors.Add(transform.GetChild(0).gameObject);
        closesDoors.Add(transform.GetChild(1).gameObject);

        //Adiciona os animators das portas.
        animatorsDoors.Add(transform.GetChild(0).GetComponent<Animator>());
        animatorsDoors.Add(transform.GetChild(1).GetComponent<Animator>());
        ArenaID = transform.GetSiblingIndex();
    }

    void Start()
    {
        closesDoors.ForEach(door => door.GetComponent<BoxCollider2D>().enabled = false);

        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        arenaBoundsCollider = GetComponent<Collider2D>();

        if (cinemachineVirtualCamera != null)
        {
            framingTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            defaultFollowTarget = cinemachineVirtualCamera.Follow;
            defaultOrthographicSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;
        }

        if (defaultFollowTarget == null && playerMoviment != null)
            defaultFollowTarget = playerMoviment.transform;

        RefreshArenaEnemies();
    }

    void Update()
    {
        animatorsDoors.ForEach(door => door.SetBool("IsActive", startedArena));
    }

    void LateUpdate()
    {
        if (targetEnemyCameraLocked)
            UpdateTargetEnemyCameraCenterPoint();

        if (!enforceLensSize || cinemachineVirtualCamera == null)
            return;

        cinemachineVirtualCamera.m_Lens.OrthographicSize = enforcedLensSize;
    }

    public void StartArena()
    {
        if (startedArena) return;

        closesDoors.ForEach(door => door.GetComponent<BoxCollider2D>().enabled = true);
        startedArena = true;

        StartArenaCameraIntro();
    }

    public void EndArena()
    {
        if (!startedArena) return;

        closesDoors.ForEach(door => door.GetComponent<BoxCollider2D>().enabled = false);
        ResetCameraTypeBehavior();
        startedArena = false;
        AuthorizeSpawn = false;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !startedArena)
        {
            timer += Time.deltaTime;

            if (timer >= timeToStartArena)
            {
                StartArena();
                timer = 0f;
            }
        }
    }

    private void StartArenaCameraIntro()
    {
        if (framingTransposer == null || cameraFocusPoint == null)
        {
            ApplyCameraTypeBehavior();
            return;
        }

        if (arenaIntroCameraCoroutine != null)
            StopCoroutine(arenaIntroCameraCoroutine);

        arenaIntroCameraCoroutine = StartCoroutine(ArenaCameraIntroCoroutine());
    }

    private IEnumerator ArenaCameraIntroCoroutine()
    {
        if (lockPlayerDuringCamera && playerMoviment != null)
            playerMoviment.canMove = false;

        yield return new WaitForSeconds(1.2f);

        Vector3 startOffset = framingTransposer.m_TrackedObjectOffset;

        Transform followTarget = cinemachineVirtualCamera != null ? cinemachineVirtualCamera.Follow : null;
        Vector3 followTargetPosition = followTarget != null
            ? followTarget.position
            : (playerMoviment != null ? playerMoviment.transform.position : transform.position);

        Vector3 targetOffset = cameraFocusPoint.position - followTargetPosition;

        float elapsed = 0f;
        while (elapsed < cameraPanTime)
        {
            elapsed += Time.deltaTime;
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, targetOffset, elapsed / cameraPanTime);
            yield return null;
        }

        framingTransposer.m_TrackedObjectOffset = targetOffset;
        animatorsDoors.ForEach(door => door.SetTrigger("TriggerStarted"));

        if (cameraStayDuration > 0f)
            yield return new WaitForSeconds(cameraStayDuration);

        animatorsDoors.ForEach(door => door.ResetTrigger("TriggerStarted"));

        elapsed = 0f;
        while (elapsed < cameraPanTime)
        {
            elapsed += Time.deltaTime;
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(targetOffset, startOffset, elapsed / cameraPanTime);
            yield return null;
        }

        framingTransposer.m_TrackedObjectOffset = startOffset;

        if (arenaType == ArenaTypes.StayCamera)
            yield return StartCoroutine(PanToStayCenterCoroutine());

        ApplyCameraTypeBehavior();

        if (lockPlayerDuringCamera && playerMoviment != null)
            playerMoviment.canMove = true;

        arenaIntroCameraCoroutine = null;
        AuthorizeSpawn = true;
    }

    private void ApplyCameraTypeBehavior()
    {
        switch (arenaType)
        {
            case ArenaTypes.DistanceCamera:
                if (cinemachineVirtualCamera != null)
                {
                    float distanceTargetSize = defaultOrthographicSize + distanceCameraLensIncrease;
                    StartLensTransition(distanceTargetSize, true);

                    if (GameManager.instance != null)
                        GameManager.instance.isCameraCorrected = false;
                }
                break;

            case ArenaTypes.StayCamera:
                if (cinemachineVirtualCamera != null)
                {
                    Transform stayTarget = stayCameraCenterPoint != null ? stayCameraCenterPoint : null;
                    if (stayTarget != null)
                    {
                        cinemachineVirtualCamera.Follow = stayTarget;
                        if (framingTransposer != null)
                            framingTransposer.m_TrackedObjectOffset = Vector3.zero;
                    }
                    else
                    {
                        cinemachineVirtualCamera.Follow = null;
                    }
                    stayCameraLocked = true;
                }
                break;

            case ArenaTypes.TargetEnemyCamera:
                if (cinemachineVirtualCamera != null)
                {
                    EnsureTargetEnemyCameraCenterPoint();
                    RefreshArenaEnemies();
                    UpdateTargetEnemyCameraCenterPoint();

                    cinemachineVirtualCamera.Follow = targetEnemyCameraCenterPoint;
                    if (framingTransposer != null)
                        framingTransposer.m_TrackedObjectOffset = Vector3.zero;

                    targetEnemyCameraLocked = true;
                }
                break;
        }
    }

    private void ResetCameraTypeBehavior()
    {
        if (cinemachineVirtualCamera == null)
            return;

        if (stayCameraLocked || targetEnemyCameraLocked)
            cinemachineVirtualCamera.Follow = defaultFollowTarget;

        StartLensTransition(defaultOrthographicSize, false);

        if (framingTransposer != null)
            framingTransposer.m_TrackedObjectOffset = Vector3.zero;

        stayCameraLocked = false;
        targetEnemyCameraLocked = false;
    }

    private void EnsureTargetEnemyCameraCenterPoint()
    {
        if (targetEnemyCameraCenterPoint != null)
            return;

        GameObject centerPointObject = new GameObject($"{name}_TargetEnemyCameraCenter");
        targetEnemyCameraCenterPoint = centerPointObject.transform;
        targetEnemyCameraCenterPoint.position = defaultFollowTarget != null ? defaultFollowTarget.position : transform.position;
    }

    private void RefreshArenaEnemies()
    {
        arenaEnemies.Clear();

        Damage[] damagesInScene = FindObjectsByType<Damage>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Damage damage in damagesInScene)
        {
            if (IsArenaEnemyCandidate(damage))
                arenaEnemies.Add(damage);
        }
    }

    public void RegisterArenaEnemy(GameObject enemyObject)
    {
        if (enemyObject == null)
            return;

        Damage damage = enemyObject.GetComponent<Damage>();
        if (damage == null)
            damage = enemyObject.GetComponentInChildren<Damage>();

        if (damage == null || !IsArenaEnemyCandidate(damage) || arenaEnemies.Contains(damage))
            return;

        arenaEnemies.Add(damage);

        if (targetEnemyCameraLocked)
            UpdateTargetEnemyCameraCenterPoint();
    }

    private bool IsArenaEnemyCandidate(Damage damage)
    {
        if (damage == null || !damage.gameObject.scene.IsValid())
            return false;

        if (playerMoviment != null && damage.gameObject == playerMoviment.gameObject)
            return false;

        if (damage.CompareTag("Player"))
            return false;

        bool isInsideArena = damage.transform.IsChildOf(transform)
            || (arenaBoundsCollider != null && arenaBoundsCollider.bounds.Contains(damage.transform.position));

        if (!isInsideArena)
            return false;

        return damage.CompareTag("Inimigos")
            || damage.GetComponent<PlayerPoco>() != null
            || damage.GetComponent<EnemyPathing>() != null
            || damage.GetComponent<EnemyReturnToBase>() != null;
    }

    private bool IsValidArenaEnemyTarget(Damage damage)
    {
        return IsArenaEnemyCandidate(damage)
            && damage.gameObject.activeInHierarchy
            && damage.IsAlive;
    }

    private void UpdateTargetEnemyCameraCenterPoint()
    {
        if (targetEnemyCameraCenterPoint == null)
            return;

        Vector3 fallbackPosition = playerMoviment != null ? playerMoviment.transform.position : transform.position;
        Bounds combatBounds = new Bounds(fallbackPosition, Vector3.zero);

        foreach (Damage enemy in arenaEnemies)
        {
            if (!IsValidArenaEnemyTarget(enemy))
                continue;

            combatBounds.Encapsulate(enemy.transform.position);
        }

        Vector3 centerPosition = combatBounds.center;
        centerPosition.z = fallbackPosition.z;
        targetEnemyCameraCenterPoint.position = centerPosition;
    }

    private Vector3 GetStayCameraCenterPosition()
    {
        if (stayCameraCenterPoint != null)
            return stayCameraCenterPoint.position;

        return transform.position;
    }

    private IEnumerator PanToStayCenterCoroutine()
    {
        if (framingTransposer == null)
            yield break;

        Transform followTarget = cinemachineVirtualCamera != null ? cinemachineVirtualCamera.Follow : null;
        Vector3 followTargetPosition = followTarget != null
            ? followTarget.position
            : (playerMoviment != null ? playerMoviment.transform.position : transform.position);

        Vector3 startOffset = framingTransposer.m_TrackedObjectOffset;

        float elapsed = 0f;
        while (elapsed < cameraPanTime)
        {
            elapsed += Time.deltaTime;
            Vector3 stayTargetOffset = GetStayCameraCenterPosition() - followTargetPosition;
            framingTransposer.m_TrackedObjectOffset = Vector3.Lerp(startOffset, stayTargetOffset, elapsed / cameraPanTime);
            yield return null;
        }

        framingTransposer.m_TrackedObjectOffset = GetStayCameraCenterPosition() - followTargetPosition;
    }

    private void StartLensTransition(float targetSize, bool keepEnforcedAfter)
    {
        if (cinemachineVirtualCamera == null)
            return;

        if (lensTransitionCoroutine != null)
            StopCoroutine(lensTransitionCoroutine);

        lensTransitionCoroutine = StartCoroutine(LensTransitionCoroutine(targetSize, keepEnforcedAfter));
    }

    private IEnumerator LensTransitionCoroutine(float targetSize, bool keepEnforcedAfter)
    {
        enforceLensSize = true;
        float startSize = cinemachineVirtualCamera.m_Lens.OrthographicSize;

        if (lensTransitionTime <= 0f)
        {
            enforcedLensSize = targetSize;
            cinemachineVirtualCamera.m_Lens.OrthographicSize = enforcedLensSize;
            if (!keepEnforcedAfter)
                enforceLensSize = false;

            lensTransitionCoroutine = null;
            yield break;
        }

        float elapsed = 0f;
        while (elapsed < lensTransitionTime)
        {
            elapsed += Time.deltaTime;
            enforcedLensSize = Mathf.Lerp(startSize, targetSize, elapsed / lensTransitionTime);
            yield return null;
        }

        enforcedLensSize = targetSize;
        cinemachineVirtualCamera.m_Lens.OrthographicSize = enforcedLensSize;

        if (!keepEnforcedAfter)
            enforceLensSize = false;

        lensTransitionCoroutine = null;
    }
}
