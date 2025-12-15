using UnityEngine;

public class Attack_Slime : MonoBehaviour
{
    public int attackDamage = 1;
    public Vector2 knockback = Vector2.zero;

    private void OnTriggerStay2D(Collider2D Collision)
    {
        if (Collision.CompareTag("Parry"))
        {
            Damage damage = Collision.GetComponent<Damage>();

            if (damage != null)
            {
                Vector2 flipknockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);

                // ataque ao alvo
                bool goHit = damage.Hit(attackDamage, flipknockback, transform.parent);
                if (goHit)
                {
                    Debug.Log("AtaqueInimigo");
                }
            }   
        }
    }
}
