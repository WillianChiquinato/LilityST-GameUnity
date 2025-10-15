using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Patrulha_Attack : MonoBehaviour
{
    public int attackDamage = 1;
    public Vector2 knockback = Vector2.zero;
    public GameObject componentePai;

    void Start()
    {
        componentePai = transform.parent.gameObject;
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        Damage damage = Collision.GetComponent<Damage>();

        if (damage != null)
        {
            Vector2 flipknockback = componentePai.transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            // ataque ao alvo
            bool goHit = damage.Hit(attackDamage, flipknockback);
            if (goHit)
            {
                Debug.Log("AtaqueInimigo");
            }
        }
    }
}
