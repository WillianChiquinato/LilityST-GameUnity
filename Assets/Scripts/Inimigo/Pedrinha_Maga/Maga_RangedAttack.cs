using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Maga_RangedAttack : MonoBehaviour
{
    public bool rangedAttack;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rangedAttack = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            rangedAttack = false;
        }
    }
}
