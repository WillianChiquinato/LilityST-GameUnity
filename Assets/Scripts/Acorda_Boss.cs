using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Acorda_Boss : MonoBehaviour
{
    public DroggoScript droggoScript;
    public BossFight bossFight;

    public GameObject Camera_Size;
    public GameObject CutSceneDroggo;
    public Animator Size_Camera;
    public PlayerMoviment playerMoviment;

    public bool Acordou = false;
    public bool _Combate = false;
    
    public bool Combate
    {
        get
        {
            return _Combate;
        }
        private set
        {
            _Combate = value;
            droggoScript.animator.SetBool(animationstrings.Combate, value);
        }
    }

    private void Awake() 
    {
        droggoScript = GameObject.FindObjectOfType<DroggoScript>();
        bossFight = GameObject.FindObjectOfType<BossFight>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();

        Camera_Size = GameObject.FindWithTag("MainCamera");
        Size_Camera = Camera_Size.GetComponentInChildren<Animator>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(DelayComeço());
        }
    }

    public IEnumerator DelayComeço()
    {
        playerMoviment.canMove = false;
        Acordou = true;
        droggoScript.animator.SetBool(animationstrings.Ranged, false);

        yield return new WaitForSeconds(2f);

        droggoScript.animator.SetBool(animationstrings.Acordou, true);

        yield return new WaitForSeconds(2.5f);

        bossFight.SceneDroggo = false;
        bossFight.CutSceneDroggo.SetActive(false);
        bossFight.canvasHUD.SetActive(true);
        Size_Camera.SetBool(animationstrings.Boss_Fight, true);
        Acordou = false;
        playerMoviment.canMove = true;
        this.gameObject.SetActive(false);
    }
}
