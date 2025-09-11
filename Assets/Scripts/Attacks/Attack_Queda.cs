using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Queda : MonoBehaviour
{
    public int attackDamage = 50;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerStay2D(Collider2D Collision)
    {
        Damage damage = Collision.GetComponent<Damage>();

        if (damage != null)
        {
            Vector2 flipknockback = transform.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

            // ataque ao alvo
            bool goHit = damage.hit(attackDamage, flipknockback);
            if (goHit)
            {
                //Estático.
                Collision.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
                Collision.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
                Debug.Log("AtaqueInimigo");
            }
        }
    }
}
