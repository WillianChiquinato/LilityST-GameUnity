using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using System;

public class SkillTreeUI : MonoBehaviour
{
    public ItemDescriptionCheck skillTreeCheck;

    public bool unLocked;
    public bool alternativoAtivado = false;
    [SerializeField] private Color lockedSkillColor;

    [SerializeField] private SkillTreeUI[] shouldBeUnlocked;
    [SerializeField] private SkillTreeUI[] shouldBeLocked;
    [SerializeField] private SkillTreeUI[] habilidadeAlternativa;
    
    [SerializeField] private Image skillImage;

    [Header("ItensTransicoes")]
    public Texture imagemRef;
    public String textoRef;

    void Start()
    {
        skillTreeCheck = GameObject.FindFirstObjectByType<ItemDescriptionCheck>();
        skillImage = GetComponent<Image>();
        skillImage.color = lockedSkillColor;

        if (habilidadeAlternativa.Length == 0)
        {
            alternativoAtivado = true;
        }
    }

    public void UnlockSkillSlot()
    {
        for (int i = 0; i < shouldBeUnlocked.Length; i++)
        {
            if (!shouldBeUnlocked[i].unLocked)
            {
                Debug.Log("Skill travada: Falta desbloquear uma habilidade necessária.");
                return;
            }
        }

        for (int i = 0; i < shouldBeLocked.Length; i++)
        {
            if (shouldBeLocked[i].unLocked)
            {
                Debug.Log("Skill travada: Habilidade do lado oposto já desbloqueada.");
                return;
            }
        }


        foreach (SkillTreeUI habilidade in habilidadeAlternativa)
        {
            if (habilidade.unLocked)
            {
                if (alternativoAtivado)
                {
                    Debug.Log("Skill travada: Mais de uma habilidade alternativa ativada.");
                    return;
                }
                alternativoAtivado = true;
            }
        }


        if (!alternativoAtivado)
        {
            Debug.Log("Skill travada: Nenhuma habilidade alternativa ativada.");
            return;
        }

        unLocked = true;
        skillImage.color = Color.green;
        Debug.Log("Habilidade desbloqueada com sucesso.");
    }
}
