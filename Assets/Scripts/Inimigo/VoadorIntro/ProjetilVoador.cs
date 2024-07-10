using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjetilVoador : MonoBehaviour
{
    public PlayerMoviment player;
    public Voador_Moviment voador_Moviment;
    private Rigidbody2D rb;
    public Transform rbVoador;
    public TrailRenderer tr;
    public float forcaProjetil;

    public float timer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rbVoador = rb.GetComponentInChildren<Transform>();
        player = GameObject.FindAnyObjectByType<PlayerMoviment>();
        voador_Moviment = GameObject.FindAnyObjectByType<Voador_Moviment>();

        Vector3 direcao = player.transform.position - transform.position;
        rb.velocity = new Vector2(direcao.x, direcao.y).normalized * forcaProjetil;
    
        float rotacao = Mathf.Atan2(-direcao.y, -direcao.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, rotacao + 90);

        tr.emitting = true;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;

        if(timer > 10) 
        {
            Destroy(gameObject);    
        }

        if (voador_Moviment.transform.localScale.x == 1)
        {
            rbVoador.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            rbVoador.transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player")) 
        {
            Destroy(gameObject);  
        }
    }
}
