using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DialogoTriggerRobert
{
    public string nome;
    public Sprite icone;
}

[System.Serializable]
public class DialogoTextoRobert
{
    public DialogoTriggerRobert caracter;
    [TextArea(3, 10)]
    public string linhaTexto;
    public bool isLility = false;
}

[System.Serializable]
public class DialogosRobert
{
    public List<DialogoTextoRobert> dialogoTextos = new List<DialogoTextoRobert>();
}

public class Dialogo_LilityPqna : MonoBehaviour
{
    public DialogosRobert dialogos;
    public PlayerBebe_Moviment playerBebe;
    public Animator animator;

    public TextMeshPro textoPress;

    private void Start()
    {
        playerBebe = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        animator = GetComponent<Animator>();
    }

    public void TriggerDialogo()
    {
        Dialogos_Manager2.dialogos_Manager.StartDialogos(dialogos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        textoPress.text = "Press [E]";
        if (collision.CompareTag("Player") && playerBebe.entrar == true)
        {
            TriggerDialogo();
            playerBebe.camerafollowObject.transposer.m_TrackedObjectOffset = new Vector3(0, 1f, 0);
            // animator.SetBool(animationstrings.InicioDialogo, true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        textoPress.text = "";
    }
}
