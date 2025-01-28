using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractableObjectMesa_Robert : CollidableObjects
{
    public Dialogos_Manager2 dialogos_Manager2;
    public PlayerBebe_Moviment playerBebe_Moviment;
    private bool z_interacted = false;
    public TextMeshPro textoPress;

    public GameObject item_robert;
    public float tempoExecucao = 0f;

    public string sceneName;
    private LevelTransicao transicao;

    protected override void Start()
    {
        base.Start();
        dialogos_Manager2 = GameObject.FindFirstObjectByType<Dialogos_Manager2>();
        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();

        item_robert.SetActive(false);
    }

    // override ele sobreescreve algo, no caso a funcao
    protected override void OnCollider(GameObject ColliderObject)
    {
        if (playerBebe_Moviment.entrar)
        {
            OnInteract();
        }
    }

    protected override void Update()
    {
        base.Update();
        if (z_interacted)
        {
            tempoExecucao += Time.deltaTime;
            if (tempoExecucao > 2f)
            {
                transicao.Transicao(sceneName);
            }
        }
    }

    private void OnInteract()
    {
        if (!z_interacted)
        {
            z_interacted = true;
            paredes_pretas.SetActive(true);
            playerBebe_Moviment.playerInput.enabled = false;
            playerBebe_Moviment.camerafollowObject.transposer.m_TrackedObjectOffset = new Vector3(0, 1f, 0);
            item_robert.SetActive(true);
            // SavePoint.CheckpointPosition = new Vector2(-89.49f, 31.84f);

            Debug.Log("Interagindo com " + name);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        textoPress.text = "Press [E]";
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        textoPress.text = "";
    }
}
