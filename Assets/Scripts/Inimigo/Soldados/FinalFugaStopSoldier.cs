using System.Collections.Generic;
using UnityEngine;

public class FinalFugaStopSoldier : MonoBehaviour
{
    public List<EnemyPathing> enemyPathing = new List<EnemyPathing>();
    private bool playerExitedLeft = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Inimigos"))
        {
            EnemyPathing enemy = collision.GetComponent<EnemyPathing>();
            if (enemy != null && !enemyPathing.Contains(enemy))
            {
                enemyPathing.Add(enemy);
                if (playerExitedLeft)
                {
                    enemy.canMove = false;
                    enemy.speed = enemy.minSpeed;
                    enemy.animator.SetBool("IdleState", true);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.position.x < transform.position.x)
            {
                playerExitedLeft = false;
                foreach (EnemyPathing enemy in enemyPathing)
                {
                    if (enemy != null)
                    {
                        enemy.animator.SetBool("IdleState", false);
                    }
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.transform.position.x > transform.position.x)
            {
                playerExitedLeft = false;
                foreach (EnemyPathing enemy in enemyPathing)
                {
                    if (enemy != null)
                    {
                        enemy.animator.SetBool("IdleState", false);
                    }
                }
            }
            else
            {
                playerExitedLeft = true;
                foreach (EnemyPathing enemy in enemyPathing)
                {
                    if (enemy != null)
                    {
                        enemy.speed = enemy.minSpeed;
                        enemy.animator.SetBool("IdleState", true);
                    }
                }
            }
        }

        if (collision.CompareTag("Inimigos"))
        {
            EnemyPathing enemy = collision.GetComponent<EnemyPathing>();
            if (enemy != null && enemyPathing.Contains(enemy))
            {
                enemyPathing.Remove(enemy);
            }
        }
    }
}
