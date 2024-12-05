using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BossFight : MonoBehaviour
{
    private CinemachineConfiner2D confiner;
    public GameObject CutSceneDroggo;
    public bool SceneDroggo;
    public GameObject Camera_Referencia;
    public PolygonCollider2D polygonCollider2;
    public GameObject LimiteBoss;

    public GameObject canvasHUD;


    void Awake()
    {
        canvasHUD = GameObject.FindGameObjectWithTag("Sumir");
        confiner = FindObjectOfType<CinemachineConfiner2D>();
        canvasHUD.SetActive(true);

        Camera_Referencia = GameObject.FindWithTag("LimiteMap");
        polygonCollider2 = Camera_Referencia.GetComponentInChildren<PolygonCollider2D>();

        LimiteBoss = GameObject.FindWithTag("Finish");
        LimiteBoss.SetActive(false);

        CutSceneDroggo = GameObject.FindWithTag("CutScene");
    }

    void Update()
    {
        confiner.InvalidateCache();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(AtualizarBossFight());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SceneDroggo = true;
            canvasHUD.SetActive(false);
        }
    }

    IEnumerator AtualizarBossFight()
    {
        Vector2[] newPoints = polygonCollider2.points;

        yield return new WaitForSeconds(4f);

        newPoints[0] = new Vector2(327, -31.9f);
        newPoints[1] = new Vector2(288.8f, -31.9f);
        newPoints[2] = new Vector2(288.8f, -49.5f);
        newPoints[3] = new Vector2(327, -49.5f);
        polygonCollider2.points = newPoints;
        LimiteBoss.SetActive(true);
        confiner.InvalidateCache();
    }
}
