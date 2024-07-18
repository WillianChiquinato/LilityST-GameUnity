using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Rigidbody2D Arrow;
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public float ForceArrow;
    public Transform ShotPoint;
    public Rigidbody2D NewArrow;
    public Animator animator;


    //Caminho da Flecha
    public GameObject shotPoint;
    public GameObject point;
    public GameObject posicaoGO;
    GameObject[] points;
    public int numeroDePoints;
    public float SpaceEntreEles;
    public bool Respawn;


    public Camera cameraArco;
    Vector2 Direcao;
    public Transform FollowArco;

    void Start()
    {
        gameObject.SetActive(false);
        cameraArco = FindObjectOfType<Camera>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        animator = GetComponent<Animator>();
        cinemachineVirtualCamera = GameObject.FindObjectOfType<CinemachineVirtualCamera>();

        points = new GameObject[numeroDePoints];

        for (int i = 0; i < numeroDePoints; i++)
        {
            points[i] = Instantiate(point, ShotPoint.position, Quaternion.identity);
        }
    }


    void Update()
    {


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

        for (int i = 0; i < numeroDePoints; i++)
        {
            points[i].transform.position = PointPosition(i * SpaceEntreEles);
        }

        if (NewArrow)
        {
            playerMoviment.animacao.SetBool(animationstrings.Powers, false);
            animator.SetBool(animationstrings.PowersBraco, false);
            StartCoroutine(delayAnimation());
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
        }
        else
        {
            playerMoviment.transform.localScale = new Vector3(-1, 1, 1);
            playerMoviment._IsRight = false;
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

    private IEnumerator delayAnimation()
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
            }
        }
    }
}
