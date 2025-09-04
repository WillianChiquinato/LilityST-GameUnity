using System.Collections.Generic;
using UnityEngine;

public class ColiderJump : MonoBehaviour
{
    public List<EnemyPathing> enemyPathing = new List<EnemyPathing>();

    void Update()
    {
        if (enemyPathing != null)
        {
            foreach (EnemyPathing enemy in enemyPathing)
            {
                if (enemy != null)
                {
                    enemy.jumpForce = 13f;
                }
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Inimigos"))
        {
            Debug.Log("Soldado esta seguindo Lility");
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
