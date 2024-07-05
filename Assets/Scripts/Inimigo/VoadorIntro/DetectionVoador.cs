using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionVoador : MonoBehaviour
{
    public List<Collider2D> detectColliders = new List<Collider2D>();
    Collider2D Col;
    public Voador_Moviment voador_Moviment;
    public bool perseguindo = false;

    private void Awake() 
    {
        Col = GetComponent<Collider2D>();
        voador_Moviment = GameObject.FindAnyObjectByType<Voador_Moviment>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        detectColliders.Add(collision);
        perseguindo = true;
    }

    private void OnTriggerExit2D(Collider2D collision) 
    {
        detectColliders.Remove(collision);
        perseguindo = false;
    }
}
