using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Rigidbody2D Arrow;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    [SerializeField]
    public CinemachineFramingTransposer transposer;
    public float ForceArrow;
    public Transform ShotPoint;
    public Rigidbody2D NewArrow;
    public Animator animator;


    //Caminho da Flecha
    public GameObject shotPoint;
    public GameObject point;
    public GameObject posicaoGO;
    public GameObject[] points;
    public int numeroDePoints;
    public float SpaceEntreEles;
    public bool Respawn;


    public Camera cameraArco;
    Vector2 Direcao;
    public Transform FollowArco;
    public Vector3 newOffset;

    //Virada da camera
    public float targetOffsetX = 2f; // O valor alvo para o offset X
    public float transitionDuration = 2f; // Duração da transição em segundos

    private float initialOffsetX;
    private float transitionStartTime;
    public bool bodyCamera;

    void Start()
    {
        gameObject.SetActive(false);
        cameraArco = FindObjectOfType<Camera>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        animator = GetComponent<Animator>();
        cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
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

        Vector2 BowPosition = transform.position;
        Vector2 mousePosition = cameraArco.ScreenToWorldPoint(Input.mousePosition);
        Direcao = mousePosition - BowPosition;
        transform.right = Direcao;

        if (Input.GetMouseButtonDown(0))
        {
            if (playerMoviment.Atirar == true)
            {
                //Projetil do voador
                Shoot();
            }
        }


        if (NewArrow)
        {
            StartCoroutine(delayAnimation());
            bodyCamera = false;
            newOffset = new Vector3(0, 0, 0);
            transposer.m_TrackedObjectOffset = newOffset;
            playerMoviment.animacao.SetBool(animationstrings.Powers, false);
            animator.SetBool(animationstrings.PowersBraco, false);
            Time.timeScale = 1f;
            playerMoviment.tempo = false;
            playerMoviment.elapsedTime = 0f;
            NewArrow = null;
            Destroy(NewArrow, 2f);
        }

        if (Direcao.x >= 1)
        {
            playerMoviment.transform.localScale = new Vector3(1, 1, 1);
            playerMoviment._IsRight = true;

            if (bodyCamera)
            {
                newOffset = new Vector3(newXOffset, 0, 0);
                transposer.m_TrackedObjectOffset = newOffset;
            }
        }
        else
        {
            playerMoviment.transform.localScale = new Vector3(-1, 1, 1);
            playerMoviment._IsRight = false;

            if (bodyCamera)
            {
                newOffset = new Vector3(-newXOffset, 0, 0);
                transposer.m_TrackedObjectOffset = newOffset;
            }
        }
    }

    public void Shoot()
    {
        Respawn = true;
        NewArrow = Instantiate(Arrow, ShotPoint.position, ShotPoint.rotation);
        if (playerMoviment.transform.localScale.x == 1)
        {
            NewArrow.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            NewArrow.transform.localScale = new Vector3(-1, 1, 1);
        }

        NewArrow.velocity = NewArrow.transform.right * ForceArrow;
    }

    Vector2 PointPosition(float T)
    {
        Vector2 position = (Vector2)ShotPoint.position + (Direcao.normalized * ForceArrow * T) + 0.5f * Physics2D.gravity * (T * T);
        return position;
    }

    public IEnumerator delayAnimation()
    {
        yield return new WaitForSeconds(0.4f);

        gameObject.SetActive(false);

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
