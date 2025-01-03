using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogoTriggerCaracter
{
    public string nome;
    public Sprite icone;
    public bool isUpDialog;
}

[System.Serializable]
public class DialogoTexto
{
    public DialogoTriggerCaracter caracter;
    [TextArea(3, 10)]
    public string linhaTexto;
}

[System.Serializable]
public class Dialogos
{
    public List<DialogoTexto> dialogoTextos = new List<DialogoTexto>();
}

public class Dialogo_Trigger : MonoBehaviour
{
    public Dialogos dialogos;
    public PlayerMoviment playerMoviment;
    public Animator animator;

    private void Start()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
        animator = GetComponent<Animator>();
    }

    public void TriggerDialogo()
    {
        Dialogos_Manager.dialogos_Manager.StartDialogos(dialogos);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && playerMoviment.entrar == true)
        {
            TriggerDialogo();
            animator.SetBool(animationstrings.InicioDialogo, true);   
        }
    }
}
