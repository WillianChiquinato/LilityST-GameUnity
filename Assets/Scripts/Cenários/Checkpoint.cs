using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public SavePoint savePoint;
    public PlayerMoviment playerMoviment;

    void Awake()
    {
        savePoint = GameObject.FindAnyObjectByType<SavePoint>();
        playerMoviment = GameObject.FindAnyObjectByType<PlayerMoviment>();
        // SavePoint.CinemaVirtual = GameObject.FindGameObjectWithTag("EditorOnly").GetComponent<CinemachineVirtualCamera>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            //Todas as questões sobre "Salvar" no checkpoint, ou reset 
            SavePoint.CheckpointPosition = transform.position;
            SavePoint.CheckpointAnim = true;
            SavePoint.CheckpointAnim2 = true;

            var lens = SavePoint.CinemaVirtual.m_Lens;
            // Alterando o OrthographicSize
            lens.OrthographicSize = 7f;
            SavePoint.CinemaVirtual.m_Lens = lens;

            // playerMoviment.potion_Script.potionInt = playerMoviment.potion_Script.maxPotionsInt;
            // playerMoviment.DamageScript.Health = playerMoviment.DamageScript.maxHealth;
            Debug.Log("Checkpoint");
        }
    }
}