using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Camera cam;
    public Transform PersoSeguir;

    // object ponto inicial do background
    Vector2 startingPosition;

    // VerticalWrapMode para o parallax
    float starting;

    // distancia da camera em relação ao start do parallax, criou uma arrow function apenas para ser mais rapido, mas poderia colocar no update
    Vector2 MoveSinceStart => (Vector2) cam.transform.position - startingPosition;

    float distanciaDoTargetZ => transform.position.z - PersoSeguir.transform.position.z;
    // nao entendi
    float ClippingPlane => (cam.transform.position.z + (distanciaDoTargetZ > 0 ? cam.farClipPlane : cam.nearClipPlane));

    float parallaxFactor => Mathf.Abs(distanciaDoTargetZ / ClippingPlane);

    void Start()
    {
        cam = GameObject.FindObjectOfType<Camera>();
        PersoSeguir = GameObject.FindObjectOfType<PlayerMoviment>().GetComponentInChildren<Transform>();

        startingPosition = transform.position;
        starting = transform.position.z;
    }

    void Update()
    {
        Vector2 novaPosicao = startingPosition + MoveSinceStart * parallaxFactor;
        
        // para mudar a velocidade quando o perso andar no eixa X e Y
        transform.position = new Vector3(novaPosicao.x, novaPosicao.y, starting);
    }
}
