using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionVoador : MonoBehaviour
{
    public List<Collider2D> detectColliders = new List<Collider2D>();
    Collider2D Col;
    public bool perseguindo = false;

    private void Awake()
    {
        Col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("Parry"))
        {
            detectColliders.Add(collision);
            //Ver mais pra frente
            perseguindo = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        detectColliders.Remove(collision);
    }
}
