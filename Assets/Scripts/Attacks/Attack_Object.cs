using UnityEngine;

public class Attack_Object : MonoBehaviour
{
    public int attackDamage = 1;
    public Vector2 knockback = Vector2.zero;

    private void OnCollisionEnter2D(Collision2D Collision)
    {
        Damage damage = Collision.collider.GetComponent<Damage>();

        if (damage != null)
        {
            Vector2 direction = (Collision.transform.position - transform.position).normalized;
            Vector2 calculatedKnockback = new Vector2(direction.x * knockback.x, direction.y * knockback.y);

            // ataque ao alvo
            bool goHit = damage.Hit(attackDamage, calculatedKnockback, transform);
            if (goHit)
            {
                Debug.Log("AtaqueInimigo");
            }
        }
    }
}
