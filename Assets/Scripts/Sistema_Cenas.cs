using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

public class Sistema_Cenas : MonoBehaviour
{
    public GameObject Camera_Size;
    public CinemachineVirtualCamera Size_Camera;
    public PlayerMoviment player;
    private LevelTransicao transicao;
    public string sceneName;

    void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMoviment>();
        transicao = GameObject.FindObjectOfType<LevelTransicao>();
        Camera_Size = GameObject.FindWithTag("MainCamera");
        Size_Camera = Camera_Size.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void OnTriggerEnter2D(Collider2D collisaoEnter)
    {
        if (collisaoEnter.CompareTag("Player"))
        {
            transicao.Transicao(sceneName);
            Debug.Log("Esta na cena " + sceneName);

            //Arrumar depois, nao esta manutenivel (Colocar alguma logica para cada saida)
            if (sceneName == "NPCs")
            {
                SavePoint.CheckpointPosition = new Vector3(-72.84f, 28.74f, 0f);
            }
            else if (sceneName == "Green_phase")
            {
                SavePoint.CheckpointPosition = new Vector3(-80.84f, 28.74f, 0f);
            }
        }
    }
}
