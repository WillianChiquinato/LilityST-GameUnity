using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public SavePoint savePoint;
    public PlayerMoviment playerMoviment;

    void Awake()
    {
        savePoint = GameObject.FindAnyObjectByType<SavePoint>();
        playerMoviment = GameObject.FindAnyObjectByType<PlayerMoviment>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SavePoint.CheckpointPosition = transform.position;
            SavePoint.CheckpointAnim = true;
            SavePoint.CheckpointAnim2 = true;
            Debug.Log("Salve");
        }
    }
}