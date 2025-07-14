using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class Bow : MonoBehaviour
{
    [Header("References")]
    public PlayerArco playerArco;
    public PlayerMoviment playerMoviment;
    public Rigidbody2D Arrow;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField]
    public CinemachineFramingTransposer transposer;
    public float ForceArrow;
    public Transform ShotPoint;
    public Rigidbody2D NewArrow;

    //Caminho da Flecha
    public GameObject point;
    public GameObject posicaoGO;
    public GameObject[] points;
    public int numeroDePoints;
    public float SpaceEntreEles;
    public bool Respawn;

    public Camera cameraArco;
    public Vector2 offset;
    [HideInInspector]
    public Vector2 Direcao;
    public Transform FollowArco;
    public Vector3 newOffset;


    [Header("Virada da camera")]
    //Virada da camera
    public float targetOffsetX = 2f;
    public float transitionDuration = 2f;

    private float initialOffsetX;
    private float transitionStartTime;
    public bool bodyCamera;

    [Header("Partes do corpo para animação")]
    public Transform tronco;
    public Transform cabeca;
    public Transform tanga;
    public Transform bracoDireito;
    public Transform bracoEsquerdo;
    public Transform ArcoAnim;

    void Start()
    {
        playerArco = GameObject.FindFirstObjectByType<PlayerArco>();
        playerArco.gameObject.SetActive(false);
        gameObject.SetActive(false);

        cameraArco = FindFirstObjectByType<Camera>();
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        cinemachineVirtualCamera = GameObject.FindFirstObjectByType<CinemachineVirtualCamera>();
        transposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

        points = new GameObject[numeroDePoints];

        for (int i = 0; i < numeroDePoints; i++)
        {
            points[i] = Instantiate(point, ShotPoint.position, Quaternion.identity);
            points[i].gameObject.SetActive(false);
        }
    }

    void Update()
    {
        // Calcula o tempo decorrido desde o início da transição
        float elapsedTime = Time.time - transitionStartTime;
        // Calcula a fração completada da transição
        float t = elapsedTime / transitionDuration;
        // Faz a interpolação linear do offset X
        float newXOffset = Mathf.Lerp(initialOffsetX, targetOffsetX, t);

        for (int i = 0; i < numeroDePoints; i++)
        {
            points[i].transform.position = PointPosition(i * SpaceEntreEles);
            points[i].gameObject.SetActive(true);
        }

        if (playerMoviment.transform.localScale.x == 1)
        {
            this.gameObject.transform.localScale = new Vector3(1, 1, 1);
            posicaoGO.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            this.gameObject.transform.localScale = new Vector3(-1, -1, 1);
            posicaoGO.transform.localScale = new Vector3(-1, 1, 1);
        }

        // Logica para arrumar o arco
        Vector3 mousePositionArco = cameraArco.ScreenToWorldPoint(Input.mousePosition);
        Vector2 BowPosition = transform.position;
        Vector2 mousePosition = new Vector2(mousePositionArco.x + offset.x, mousePositionArco.y + offset.y);
        Direcao = mousePosition - BowPosition;
        transform.right = Direcao.normalized;

        if (Input.GetMouseButtonDown(0) && !NewArrow)
        {
            if (playerMoviment.Atirar)
            {
                Shoot();
                playerMoviment.arcoEffect = false;
                tronco.GetComponent<Animator>().SetTrigger("PowerUp");
                cabeca.GetComponent<Animator>().SetTrigger("PowerUp");
                tanga.GetComponent<Animator>().SetTrigger("PowerUp");
                bracoDireito.GetComponent<Animator>().SetTrigger("PowerUp");
                bracoEsquerdo.GetComponent<Animator>().SetTrigger("PowerUp");
                ArcoAnim.GetComponent<Animator>().SetTrigger("PowerUp");
            }
        }

        if (NewArrow)
        {
            StartCoroutine(DelayAnimation());
            bodyCamera = false;
            transposer.m_TrackedObjectOffset = new Vector3(transposer.m_TrackedObjectOffset.x, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);
            Time.timeScale = 1f;
            playerMoviment.tempo = false;
            playerMoviment.elapsedTime = 0f;
            Destroy(NewArrow, 1.3f);
        }

        if (Direcao.x >= 1)
        {
            playerMoviment.transform.localScale = new Vector3(1, 1, 1);
            playerMoviment._IsRight = true;

            if (bodyCamera)
            {
                transposer.m_TrackedObjectOffset = new Vector3(newXOffset, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);
            }
        }
        else
        {
            playerMoviment.transform.localScale = new Vector3(-1, 1, 1);
            playerMoviment._IsRight = false;

            if (bodyCamera)
            {
                transposer.m_TrackedObjectOffset = new Vector3(-newXOffset, transposer.m_TrackedObjectOffset.y, transposer.m_TrackedObjectOffset.z);
            }
        }
    }

    public void Shoot()
    {
        Respawn = true;
        playerMoviment.RecuarAtirar = false;
        NewArrow = Instantiate(Arrow, ShotPoint.position, ShotPoint.rotation);
        if (playerMoviment.transform.localScale.x == 1)
        {
            NewArrow.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            NewArrow.transform.localScale = new Vector3(-1, 1, 1);
        }

        NewArrow.linearVelocity = NewArrow.transform.right * ForceArrow;
    }

    Vector2 PointPosition(float T)
    {
        Vector2 position = (Vector2)ShotPoint.position + (Direcao.normalized * ForceArrow * T) + 0.5f * Physics2D.gravity * (T * T);
        return position;
    }

    public IEnumerator DelayAnimation()
    {
        yield return new WaitForSeconds(0.4f);

        gameObject.SetActive(false);
        playerArco.gameObject.SetActive(false);
        playerMoviment.GetComponent<SpriteRenderer>().enabled = true;
        playerMoviment.animacao.SetBool(animationstrings.Powers, false);

        foreach (var DestruirCaminho in points)
        {
            Destroy(DestruirCaminho);
        }

        if (Respawn)
        {
            for (int i = 0; i < numeroDePoints; i++)
            {
                points[i] = Instantiate(point, ShotPoint.position, Quaternion.identity);
                points[i].gameObject.SetActive(false);
            }
        }
    }
}
