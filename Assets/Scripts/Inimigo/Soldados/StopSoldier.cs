using System.Collections.Generic;
using UnityEngine;

public class StopSoldier : MonoBehaviour
{
    public List<EnemyPathing> enemyPathing = new List<EnemyPathing>();

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
                    enemy.canMove = false;
                }
            }
        }
    }
}
