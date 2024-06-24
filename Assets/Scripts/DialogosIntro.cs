using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogosIntro : MonoBehaviour
{
    DialogosIntro dialogosIntro;
    PlayerMoviment playerMoviment;
    Dialogos dialogos;
    Animator animator;

    void Awake()
    {
        dialogosIntro = GameObject.FindObjectOfType<DialogosIntro>();
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        dialogos = GameObject.FindObjectOfType<Dialogos>();
        animator = dialogos.GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if(other.CompareTag("Player"))
        {
            animator.SetBool(animationstrings.Aparece, true);
        }
    }
}