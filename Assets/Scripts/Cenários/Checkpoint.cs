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
            //Todas as questões sobre "Salvar" no checkpoint, ou reset 
            SavePoint.CheckpointPosition = transform.position;
            SavePoint.CheckpointAnim = true;
            SavePoint.CheckpointAnim2 = true;
            playerMoviment.potion_Script.potionInt = playerMoviment.potion_Script.maxPotionsInt;
            playerMoviment.DamageScript.Health = playerMoviment.DamageScript.maxHealth;
            Debug.Log("Checkpoint");
        }
    }
}