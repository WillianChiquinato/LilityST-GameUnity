using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Rigidbody2D Arrow;
    public float ForceArrow;
    public Transform ShotPoint;
    public Rigidbody2D NewArrow;
    public Animator animator;


    //Caminho da Flecha
    public GameObject point;
    public GameObject posicaoGO;
    GameObject[] points;
    public int numeroDePoints;
    public float SpaceEntreEles;


    public Camera cameraArco;
    Vector2 Direcao;

    void Start()
    {
        gameObject.SetActive(false);
        cameraArco = FindObjectOfType<Camera>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        animator = GetComponent<Animator>();

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

        if (Input.GetMouseButtonDown(0) && playerMoviment.Atirar == true)
        {
            Shoot();
        }

        for (int i = 0; i < numeroDePoints; i++)
        {
            points[i].transform.position = PointPosition(i * SpaceEntreEles);
        }

        if (NewArrow)
        {
            playerMoviment.animacao.SetBool(animationstrings.Powers, false);
            animator.SetBool(animationstrings.PowersBraco, false);
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void Shoot()
    {
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
}
