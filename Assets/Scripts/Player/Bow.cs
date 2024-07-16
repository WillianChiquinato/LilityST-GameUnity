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
    

    //Caminho da Flecha
    public GameObject point;
    GameObject[] points;
    public int numeroDePoints;
    public float SpaceEntreEles;


    public Camera cameraArco;
    Vector2 Direcao;

    void Start()
    {
        gameObject.SetActive(false);
        cameraArco = FindAnyObjectByType<Camera>();
        playerMoviment = FindAnyObjectByType<PlayerMoviment>();

        points = new GameObject[numeroDePoints];
        for(int i = 0; i < numeroDePoints; i++)
        {
            points[i] = Instantiate(point, ShotPoint.position, Quaternion.identity);
        }
    }


    void Update()
    {
        Vector2 BowPosition = transform.position;
        Vector2 mousePosition = cameraArco.ScreenToWorldPoint(Input.mousePosition);
        Direcao = mousePosition - BowPosition;
        transform.right = Direcao;

        if(Input.GetMouseButtonDown(0) && playerMoviment.Atirar == true) 
        {
            Shoot();   
        }

        for(int i = 0; i < numeroDePoints; i++)
        {
            points[i].transform.position = PointPosition(i * SpaceEntreEles);
        }

        if(NewArrow) 
        {
            playerMoviment.animacao.SetBool(animationstrings.Powers, false);
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void Shoot()
    {
        NewArrow = Instantiate(Arrow, ShotPoint.position, ShotPoint.rotation);
        NewArrow.velocity = NewArrow.transform.right * ForceArrow;
    }

    Vector2 PointPosition(float T)
    {
        Vector2 position = (Vector2)ShotPoint.position + (Direcao.normalized * ForceArrow * T) + 0.5f * Physics2D.gravity * (T * T);
        return position;
    }
}
