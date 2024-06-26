using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public SavePoint savePoint;

    void Awake()
    {
        savePoint = GameObject.FindAnyObjectByType<SavePoint>();
    }

    private void OnTriggerEnter2D(Collider2D collision) 
    {
        if(collision.CompareTag("Player")) 
        {
            SavePoint.CheckpointPosition = transform.position;
        }
    }
}
