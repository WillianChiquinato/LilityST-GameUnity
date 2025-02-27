using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetIntro : MonoBehaviour
{
    [SerializeField]
    private Rigidbody2D TargetdaIntro;

    [HideInInspector]
    public Rigidbody2D TargetRB;
    public Transform TargetLocal;
    public Transform player;

    [SerializeField]
    private float VelocidadeTarget = 15f;
    public float distancia;
    private float shootTimerTarget = 2f;
    public float shootTempo;
    public bool shootTimer;

    void Awake()
    {
        player = GameObject.FindAnyObjectByType<PlayerMoviment>().GetComponentInChildren<Transform>();
    }

    void Update()
    {
        shootTempo += Time.deltaTime;


        distancia = Mathf.Abs(transform.position.x - player.position.x);

        if (distancia < 12)
        {
            shootTimer = true;

            if (shootTimer == true && shootTempo >= shootTimerTarget)
            {
                shootTempo = 0;

                TargetRB = Instantiate(TargetdaIntro, TargetLocal.transform.position, TargetLocal.transform.rotation);

                TargetRB.linearVelocity = TargetRB.transform.right * -VelocidadeTarget;
            }
        }
    }
}
