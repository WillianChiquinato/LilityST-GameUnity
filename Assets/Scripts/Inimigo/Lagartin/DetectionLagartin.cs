using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionLagartin : MonoBehaviour
{
    public List<Collider2D> detectColliders = new List<Collider2D>();
    Collider2D Col;
    public Lagartin_Moviment lagartin_Moviment;
    public bool perseguir = false;

    private void Awake() 
    {
        Col = GetComponent<Collider2D>();
        lagartin_Moviment = GameObject.FindObjectOfType<Lagartin_Moviment>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if (collision.CompareTag("Player"))
        {
            detectColliders.Add(collision);
            perseguir = true;
        }
    }
}
