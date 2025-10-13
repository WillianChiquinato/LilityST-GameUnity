using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Item_SlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemImagem;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private TextMeshProUGUI itemTexto;

    public Inventory_item item;

    //Begin and drop.
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public int slotIndex;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void UpdateInventory(Inventory_item _newItem)
    {
        item = _newItem;

        if (item != null)
        {
            itemImagem.sprite = item.itemData.Icon;
            itemTexto.text = item.stackSize > 1 ? item.stackSize.ToString() : "";
        }
        else
        {
            CleanUpSlot();
        }
    }

    public void CleanUpSlot()
    {
        item = null;
        itemImagem.sprite = originalSprite;
        itemTexto.text = "";
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Isso é um item");
    }

    //Passar o mouse exibe a descrição do item
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (item != null && item.itemData != null)
        {
            ShowToolTip();
        }
        else
        {
            Debug.Log("Item não encontrado");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ToolTipItems.Instance.HideToolTip();
    }

    public void ShowToolTip()
    {
        ToolTipItems.Instance.ShowToolTip(item.itemData, item.stackSize);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        ToolTipItems.Instance.HideToolTip();
        ItemDragGhost.Instance.Show(item.itemData.Icon, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;

        ItemDragGhost.Instance.Move(eventData.position);
        ToolTipItems.Instance.HideToolTip();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        ItemDragGhost.Instance.Hide();
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        var draggedSlot = eventData.pointerDrag?.GetComponent<Item_SlotUI>();
        if (draggedSlot == null || draggedSlot == this) return;

        if (IsCollectSlot() && draggedSlot.IsCollectSlot())
        {
            SwapCollectItems(draggedSlot.slotIndex, this.slotIndex);
        }

        ToolTipItems.Instance.HideToolTip();
    }

    private bool IsCollectSlot()
    {
        return inventory_System.instance.coletaveisItemSlot.Contains(this);
    }

    private void SwapCollectItems(int fromIndex, int toIndex)
    {
        var system = inventory_System.instance;
        var collectList = system.coletaveis;

        // Garantir que fromIndex está dentro do limite da lista
        if (fromIndex < 0 || fromIndex >= collectList.Count)
        {
            Debug.LogWarning($"Índice de origem inválido: {fromIndex}");
            return;
        }

        if (toIndex >= collectList.Count)
        {
            var itemToMove = collectList[fromIndex];
            collectList.RemoveAt(fromIndex);
            collectList.Add(itemToMove);
            Debug.Log($"Movido item de {fromIndex} para slot vazio {toIndex}");
        }
        else
        {
            var temp = collectList[fromIndex];
            collectList[fromIndex] = collectList[toIndex];
            collectList[toIndex] = temp;
            Debug.Log($"Trocado item {fromIndex} ↔ {toIndex}");
        }

        system.UpdateInventory();
    }
}
