using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;

public class UI_Descricao : MonoBehaviour
{
    [SerializeField]
    private Image itemImagem;
    
    [SerializeField]
    private TMP_Text titulo;

    [SerializeField]
    private TMP_Text descricao;

    void Awake()
    {
        ResetDescricao();
    }

    public void ResetDescricao()
    {
        this.itemImagem.gameObject.SetActive(false);
        this.titulo.text = "";
        this.descricao.text = "";
    }

    public void SetDescricao(Sprite sprite, string ItemNome, string ItemDescricao)
    {
        this.itemImagem.gameObject.SetActive(true);
        this.itemImagem.sprite = sprite;
        this.titulo.text = ItemNome;
        this.descricao.text = ItemDescricao;
    }
}
