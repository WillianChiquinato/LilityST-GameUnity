using System.Collections.Generic;
using UnityEngine;

public class ColiderJump : MonoBehaviour
{
    public List<EnemyPathing> enemyPathing = new List<EnemyPathing>();
    public float jumpForce = 13f;

    void Update()
    {
        if (enemyPathing != null)
        {
            foreach (EnemyPathing enemy in enemyPathing)
            {
                if (enemy != null)
                {
                    enemy.jumpForce = jumpForce;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Inimigos"))
        {
            EnemyPathing enemyComponent = collision.gameObject.GetComponent<EnemyPathing>();
            if (enemyComponent != null)
            {
                enemyPathing.Add(enemyComponent);
            }
            foreach (EnemyPathing enemy in enemyPathing)
            {
                if (enemy != null)
                {
                    enemy.jumpForce = 14f;
                }
            }
        }
    }
}
