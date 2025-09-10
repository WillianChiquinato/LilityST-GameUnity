using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectedFrance : MonoBehaviour
{
    public FranceMoviment goraflixMoviment;

    void Start()
    {
        goraflixMoviment = GameObject.FindFirstObjectByType<FranceMoviment>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && !goraflixMoviment.SpawnCheck)
        {
            goraflixMoviment.SpawnCheck = true;
        }
    }
}
