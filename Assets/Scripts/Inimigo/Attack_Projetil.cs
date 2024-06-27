using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Projetil : MonoBehaviour, Defender
{
    public int attackDamage = 1;
    public Vector2 knockback = Vector2.zero;
    private Rigidbody2D rb;

    PlayerMoviment player;
    DroggoScript droggoScript;

    [SerializeField]
    public float returnSpeed { get; set; } = 26f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindAnyObjectByType<PlayerMoviment>();
        droggoScript = GameObject.FindAnyObjectByType<DroggoScript>();

        Destroy(this.gameObject, 3.0f);
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Player"))
        {
            Debug.Log("EntrouPlayer");
        }
        Damage damage = Collision.GetComponent<Damage>();

        if (damage != null)
        {
            Vector2 flipknockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            // ataque ao alvo
            bool goHit = damage.hit(attackDamage, flipknockback);
            if (goHit)
            {
                Debug.Log("AtaqueProject");
            }
        }

        if (Collision.CompareTag("Parry"))
        {
            Defender(transform.localScale);
        }
    }

    public void TrocarLayer(GameObject obj, int novaLayer)
    {
        obj.layer = novaLayer;
    }

    public void Defender(Vector2 direcao)
    {
        GameObject myObject = this.gameObject;

        int novaLayer = 9;

        rb.velocity = direcao * returnSpeed;
        TrocarLayer(myObject, novaLayer);

        if(transform.localScale.x == -1) 
        {
            Debug.Log("Esquerda");
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Debug.Log("Direita");
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
