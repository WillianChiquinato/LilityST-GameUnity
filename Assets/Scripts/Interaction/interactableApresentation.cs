using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class interactableApresentation : CollidableObjects
{
    public PlayerMoviment playerMoviment;
    public GameObject ApresInput;

    public TextMeshProUGUI texto01;
    public TextMeshProUGUI texto02;
    public Texture referenciaImg;
    public RawImage imagem;

    public string GetInput;
    public bool ativo = false;

    protected override void Start()
    {
        base.Start();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        ApresInput.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.W) && ativo)
        {
            if (GetInput == "Jump")
            {
                SavePoint.JumpApres = true;
                Time.timeScale = 1f;
                playerMoviment.playerInput.enabled = true;
                ApresInput.SetActive(false);

                Destroy(this.gameObject);
                ativo = false;
            }

            if (GetInput == "WallJump")
            {
                SavePoint.WallApres = true;
                Time.timeScale = 1f;
                playerMoviment.playerInput.enabled = true;
                ApresInput.SetActive(false);

                Destroy(this.gameObject);
                ativo = false;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        ativo = true;
        Time.timeScale = 0f;
        ApresInput.SetActive(true);
        playerMoviment.playerInput.enabled = false;

        if (GetInput == "Jump")
        {
            SavePoint.JumpApres = true;
            texto01.text = "Pressione";
            texto02.text = "Para pular";
            imagem.texture = referenciaImg;
        }

        if (GetInput == "WallJump")
        {
            //
            SavePoint.WallApres = true;
            texto01.text = "Vá na parede, press W";
            texto02.text = "Para WallJump";
            imagem.texture = referenciaImg;
        }

    }
}
