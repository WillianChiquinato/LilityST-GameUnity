using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Cinemachine;

public class Sistema_Cenas : MonoBehaviour
{
    private CinemachineConfiner2D confiner;
    public GameObject Camera_Referencia;
    public PolygonCollider2D polygonCollider2;
    public GameObject Camera_Size;
    public CinemachineVirtualCamera Size_Camera;
    public PlayerMoviment player;
    private LevelTransicao transicao;
    public Sistema_Pause sistema_Pause;
    public string sceneName;
    public Vector2 SpawnPointPosition;


    void Awake()
    {
        player = GameObject.FindObjectOfType<PlayerMoviment>();
        transicao = GameObject.FindObjectOfType<LevelTransicao>();
        sistema_Pause = GameObject.FindObjectOfType<Sistema_Pause>();
        confiner = FindObjectOfType<CinemachineConfiner2D>();
        Camera_Size = GameObject.FindWithTag("MainCamera");
        Camera_Referencia = GameObject.FindWithTag("LimiteMap");
        polygonCollider2 = Camera_Referencia.GetComponentInChildren<PolygonCollider2D>();
        Size_Camera = Camera_Size.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void OnTriggerStay2D(Collider2D collisaoEnter)
    {
        if (collisaoEnter.CompareTag("Player") && player.entrar == true)
        {
            StartCoroutine(Entrando());
            transicao.Transicao(sceneName);
            Debug.Log("Esta na cena " + sceneName);
        }
    }

    IEnumerator Entrando()
    {
        Vector2[] newPoints = polygonCollider2.points;

        yield return new WaitForSeconds(1f);

        if (sceneName == "Caverna")
        {
            // Altere as posições dos pontos conforme necessário
            newPoints[0] = new Vector2(22.24086f, 8.179609f);
            newPoints[1] = new Vector2(-17.03582f, 8.305979f);
            newPoints[2] = new Vector2(-17.03582f, -4.784976f);
            newPoints[3] = new Vector2(22.24086f, -4.77769f);
            polygonCollider2.points = newPoints;
            Size_Camera.m_Lens.OrthographicSize = 5.15f;
        }

        if (sceneName == "Front_Screen")
        {
            // Altere as posições dos pontos conforme necessário
            newPoints[0] = new Vector2(133, -3.26f);
            newPoints[1] = new Vector2(21, -3.26f);
            newPoints[2] = new Vector2(21, -43.6f);
            newPoints[3] = new Vector2(133, -43.6f);
            polygonCollider2.points = newPoints;
            Size_Camera.m_Lens.OrthographicSize = 7f;
        }

        if(sceneName == "Chegada_BIG") 
        {
            // Altere as posições dos pontos conforme necessário
            newPoints[0] = new Vector2(54.59004f, 7.820715f);
            newPoints[1] = new Vector2(-19.97989f, 7.820715f);
            newPoints[2] = new Vector2(-19.97989f, -5.230483f);
            newPoints[3] = new Vector2(54.59004f, -5.230483f);
            polygonCollider2.points = newPoints;
            Size_Camera.m_Lens.OrthographicSize = 7f;
        }

        if(sceneName == "Big_Finger") 
        {
            // Altere as posições dos pontos conforme necessário
            newPoints[0] = new Vector2(0.06458855f, 22.3968f);
            newPoints[1] = new Vector2(-62.57866f, 22.3968f);
            newPoints[2] = new Vector2(-62.57866f, 6.414756f);
            newPoints[3] = new Vector2(0.06458855f, 6.414756f);
            polygonCollider2.points = newPoints;
            Size_Camera.m_Lens.OrthographicSize = 7f;
        }

        if(sceneName == "Droggo")
        {
            // Altere as posições dos pontos conforme necessário
            newPoints[0] = new Vector2(328, -31.9f);
            newPoints[1] = new Vector2(247.5f, -31.9f);
            newPoints[2] = new Vector2(247.5f, -49.5f);
            newPoints[3] = new Vector2(328, -49.5f);
            polygonCollider2.points = newPoints;
            Size_Camera.m_Lens.OrthographicSize = 7f;
        }

        player.transform.position = SpawnPointPosition;
        confiner.InvalidateCache();
    }
}
