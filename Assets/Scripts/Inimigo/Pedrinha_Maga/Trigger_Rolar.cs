using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Rolar : MonoBehaviour
{
    public bool distanciaRolar;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            distanciaRolar = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            distanciaRolar = false;
        }
    }
}
