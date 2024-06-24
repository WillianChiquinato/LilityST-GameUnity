using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIinvetárioItem : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
{
    [SerializeField]
    private RectTransform transforme;

    [SerializeField]
    private Image itemImagem;
    
    [SerializeField]
    private TMP_Text quantidadeTXT;

    [SerializeField]
    private Image bordaImagem;


    public event Action<UIinvetárioItem> OnItemClicked, OnItemDroppedOn, OnItemBegin, OnItemEndDrag, OnRightMouseClick;

    private bool empty = true;

    void Awake()
    {
        ResetData();
        OnDeselect();

    }

    public void ResetData()
    {
        this.itemImagem.gameObject.SetActive(false);
        empty = true;
    }

    public void OnDeselect()
    {
        bordaImagem.enabled = false;
    }

    public void SetData(Sprite sprite, int quantidade)
    {
        this.itemImagem.gameObject.SetActive(true);
        this.itemImagem.sprite = sprite;
        this.quantidadeTXT.text = quantidade + "";

        empty = false;
    }

    public void Select()
    {
        bordaImagem.enabled = true;
    }

    public void OnPointerClick(PointerEventData pointerEventData)
    {
        if(empty)
        {
            return;
        }
        if(pointerEventData.button == PointerEventData.InputButton.Right)
        {
            OnRightMouseClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if(empty)
        {
            return;
        }

        OnItemBegin?.Invoke(this);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnItemEndDrag?.Invoke(this);
    }

    public void OnDrop(PointerEventData eventData)
    {
        OnItemDroppedOn?.Invoke(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        
    }
}
