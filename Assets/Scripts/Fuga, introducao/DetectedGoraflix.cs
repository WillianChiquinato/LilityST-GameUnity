using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedGoraflix : MonoBehaviour
{
    public GoraflixMoviment goraflixMoviment;

    void Start()
    {
        goraflixMoviment = GameObject.FindFirstObjectByType<GoraflixMoviment>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            goraflixMoviment.atacar = true;
        }
    }
}
