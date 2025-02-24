using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZoneSlime : MonoBehaviour
{
    public List<Collider2D> detectColliders = new List<Collider2D>();
    Collider2D Col;

    private void Awake() 
    {
        Col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectColliders.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            detectColliders.Remove(collision);
        }
    }
}
