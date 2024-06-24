using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inventário : MonoBehaviour
{
    [SerializeField]
    private UIinvetárioItem itemPrefab;

    [SerializeField]
    private RectTransform ContentPainel;

    [SerializeField]
    private UI_Descricao ItemDescricao;

    [SerializeField]
    private MouseFollow mouseFollow;

    List<UIinvetárioItem> listaDeItens = new List<UIinvetárioItem>();

    public Sprite imagem, imagem2;
    public int quantidade;
    public string titulo, descricao;

    private int IndexDrgItem = -1;

    void Awake()
    {
        hide();
        mouseFollow.Toggle(false);
        ItemDescricao.ResetDescricao();
    }

    public void InventoryUI(int TamanhoInventário)
    {
        for (int i = 0; i < TamanhoInventário; i++)
        {
            UIinvetárioItem uiItem = Instantiate(itemPrefab, Vector3.zero, Quaternion.identity);
            uiItem.transform.SetParent(ContentPainel);
            listaDeItens.Add(uiItem);

            uiItem.OnItemClicked += HandleItemSelect;
            uiItem.OnItemBegin += HandleBegin;
            uiItem.OnItemDroppedOn += HandleSwap;
            uiItem.OnItemEndDrag += HandleEndDrag;
            uiItem.OnRightMouseClick += HandleShowItensAction;
        }
    }

    private void HandleShowItensAction(UIinvetárioItem item)
    {

    }

    private void HandleEndDrag(UIinvetárioItem item)
    {
        mouseFollow.Toggle(false);
    }

    private void HandleSwap(UIinvetárioItem item)
    {
        int index = listaDeItens.IndexOf(item);
        if(index == -1)
        {
            mouseFollow.Toggle(false);
            IndexDrgItem = -1;
            return;
        }
        listaDeItens[IndexDrgItem].SetData(index == 0 ? imagem : imagem2, quantidade);
        listaDeItens[index].SetData(IndexDrgItem == 0 ? imagem : imagem2, quantidade);
        mouseFollow.Toggle(false);
    }

    private void HandleBegin(UIinvetárioItem item)
    {
        int index = listaDeItens.IndexOf(item);
        if(index == -1)
        {
            return;
        }

        IndexDrgItem = index;

        mouseFollow.Toggle(true);
        mouseFollow.SetData(index == 0 ? imagem : imagem2, quantidade);
    }

    private void HandleItemSelect(UIinvetárioItem item)
    {
        ItemDescricao.SetDescricao(imagem, titulo, descricao);
        listaDeItens[0].Select();
    }

    public void show()
    {
        gameObject.SetActive(true);
        ItemDescricao.ResetDescricao();

        listaDeItens[0].SetData(imagem, quantidade);
        listaDeItens[1].SetData(imagem, quantidade);
    }

    public void hide()
    {
        gameObject.SetActive(false);
    }
}
