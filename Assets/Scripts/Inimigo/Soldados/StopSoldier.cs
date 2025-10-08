using System.Collections.Generic;
using UnityEngine;

public class StopSoldier : MonoBehaviour
{
    public List<EnemyPathing> enemyPathing = new List<EnemyPathing>();
    public bool isPlaying = false;
    public GameObject playerDetect;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Inimigos"))
        {
            EnemyPathing enemyComponent = collision.gameObject.GetComponent<EnemyPathing>();
            if (enemyComponent != null)
            {
                enemyPathing.Add(enemyComponent);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (EnemyPathing enemy in enemyPathing)
            {
                if (enemy != null)
                {
                    enemy.distancePlayerYBool = false;
                }
            }
            isPlaying = true;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Inimigos"))
        {
            foreach (EnemyPathing enemy in enemyPathing)
            {
                if (enemy != null)
                {
                    enemy.distancePlayerYBool = true;
                    enemy.distancePlayer = 0f;
                }
            }
            enemyPathing.Clear();
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            if (playerDetect != null && playerDetect.GetComponent<StopSoldierPlayerDetect>().isPlaying)
            {
                isPlaying = false;
            }
        }
    }

    void Update()
    {
        if (!isPlaying)
        {
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
