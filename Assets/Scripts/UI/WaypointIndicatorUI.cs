using UnityEngine;
using UnityEngine.UI;

public class WaypointIndicatorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconUI;
    public Image arrowUI;
    public Vector3 offsetIcon;

    private int currentFrame = 0;
    private float frameTimer = 0f;

    [Header("Arrow Settings")]
    public float arrowMargin = 50f;

    public Transform target;

    private Camera mainCamera;
    public float maxDistance = 50f;
    public float minDistance = 0f;

    void Start()
    {
        mainCamera = Camera.main;
        if (target == null)
        {
            Debug.LogWarning("WaypointIndicatorUI: No target assigned!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (target == null)
        {
            // Se o alvo foi destruído, remove o indicador.
            WaypointManager.Instance.RemoveTarget(target);
            Destroy(gameObject);
            return;
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        bool isOffscreen = screenPos.z < 0 ||
                           screenPos.x < 0 || screenPos.x > Screen.width ||
                           screenPos.y < 0 || screenPos.y > Screen.height;

        iconUI.gameObject.SetActive(isOffscreen);
        arrowUI.gameObject.SetActive(isOffscreen);

        if (isOffscreen)
        {
            Vector3 clampedPos = screenPos;
            Vector3 clampedPosIcon = screenPos;

            clampedPos += offsetIcon;

            float halfWidth = iconUI.rectTransform.rect.width / 2.6f;
            float halfHeight = iconUI.rectTransform.rect.height / 2.6f;

            clampedPosIcon.x = Mathf.Clamp(clampedPosIcon.x, arrowMargin + halfWidth, Screen.width - arrowMargin - halfWidth);
            clampedPosIcon.y = Mathf.Clamp(clampedPosIcon.y, arrowMargin + halfHeight, Screen.height - arrowMargin - halfHeight);
            clampedPos.x = Mathf.Clamp(clampedPos.x, arrowMargin, Screen.width - arrowMargin);
            clampedPos.y = Mathf.Clamp(clampedPos.y, arrowMargin, Screen.height - arrowMargin);

            arrowUI.rectTransform.position = clampedPos;
            iconUI.rectTransform.position = clampedPosIcon;

            Vector3 dir = (screenPos - clampedPos).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowUI.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 80f);

            //Aumenta a escala da seta quando o alvo está mais perto.
            float distance = Vector3.Distance(WaypointManager.Instance.player.position, target.position);
            float t = Mathf.InverseLerp(maxDistance, minDistance, distance);

            float scale = Mathf.Lerp(0.7f, 1.4f, t);
            iconUI.rectTransform.localScale = Vector3.one * scale;
            iconUI.GetComponent<Animator>().speed = Mathf.Lerp(1f, 4f, t);
        }
    }
}