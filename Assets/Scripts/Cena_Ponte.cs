using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cena_Ponte : MonoBehaviour
{
    public GameObject Camera_Size;
    public PlayerMoviment player;
    private LevelTransicao transicao;
    public string sceneName;
    public bool Ponte;

    void Awake()
    {
        player = GameObject.FindFirstObjectByType<PlayerMoviment>();
        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        Camera_Size = GameObject.FindWithTag("MainCamera");
    }

    void Update()
    {
        if (Ponte)
        {
            StartCoroutine(TimerPonte());
        }
    }

    public void OnTriggerStay2D(Collider2D collisaoEnter)
    {
        if (collisaoEnter.CompareTag("Player"))
        {
            Ponte = true;
            player.DamageScript.Health++;
        }
    }

    IEnumerator TimerPonte()
    {
        player.canMove = false;
        player.playerInput.enabled = false;
        yield return new WaitForSeconds(0.7f);
        player.transform.localScale = new Vector3(1, 1, 1);
    }
}
