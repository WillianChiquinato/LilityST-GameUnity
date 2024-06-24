using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    public List<Collider2D> detectColliders = new List<Collider2D>();
    Collider2D Col;

    private void Awake() 
    {
        Col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        detectColliders.Add(collision);
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        detectColliders.Remove(collision);
    }
}
