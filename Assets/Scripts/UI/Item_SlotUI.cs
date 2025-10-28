using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum SlotType
{
    Comum,
    Coletavel,
    Quest
}

public class Item_SlotUI : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemImagem;
    [SerializeField] private Sprite originalSprite;
    [SerializeField] private TextMeshProUGUI itemTexto;

    public Inventory_item item;

    //Begin and drop.
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public SlotType slotType;
    public int slotIndex;

    [Header("Animações")]
    private Vector3 originalScale;
    private Vector3 targetScale;
    private Vector3 hoverScale = new Vector3(1.05f, 1.05f, 1.05f);
    private Vector3 selectedScale = new Vector3(1.15f, 1.15f, 1.15f);
    private Vector3 originalPosition;
    private Vector3 hoverOffset = new Vector3(0, 10f, 0);
    private Coroutine animRoutine;

    public System.Action<SlotType, int> OnSlotClicked;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalScale = transform.localScale;
        originalPosition = transform.localPosition;
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

    public void OnPointerEnter(PointerEventData eventData)
    {
        // if (item == null) return;
        // StartAnimation(originalPosition + hoverOffset, hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // StartAnimation(originalPosition, originalScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (item == null) return;
        // if (animRoutine != null) StopCoroutine(animRoutine);
        // animRoutine = StartCoroutine(PulseEffect());

        OnSlotClicked?.Invoke(slotType, slotIndex);
    }

    private void StartAnimation(Vector3 targetPos, Vector3 targetScale)
    {
        if (animRoutine != null) StopCoroutine(animRoutine);
        animRoutine = StartCoroutine(AnimateTransform(targetPos, targetScale));
    }

    private IEnumerator AnimateTransform(Vector3 targetPos, Vector3 targetScale)
    {
        float duration = 0.15f;
        float time = 0f;

        Vector3 startPos = transform.localPosition;
        Vector3 startScale = transform.localScale;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime; // usa tempo independente do Time.timeScale
            float t = time / duration;

            transform.localPosition = Vector3.Lerp(startPos, targetPos, t);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null;
        }

        transform.localPosition = targetPos;
        transform.localScale = targetScale;
    }

    private IEnumerator PulseEffect()
    {
        Vector3 startScale = transform.localScale;
        Vector3 peakScale = selectedScale;

        float duration = 0.15f;
        float time = 0f;

        // Expande
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(startScale, peakScale, t);
            yield return null;
        }

        time = 0f;
        Vector3 endScale = hoverScale;
        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            float t = time / duration;
            transform.localScale = Vector3.Lerp(peakScale, endScale, t);
            yield return null;
        }

        transform.localScale = endScale;
        animRoutine = null;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null) return;

        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        ItemDragGhost.Instance.Show(item.itemData.Icon, eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null) return;

        ItemDragGhost.Instance.Move(eventData.position);
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
            return;
        }

        if (toIndex >= collectList.Count)
        {
            var itemToMove = collectList[fromIndex];
            collectList.RemoveAt(fromIndex);
            collectList.Add(itemToMove);
        }
        else
        {
            var temp = collectList[fromIndex];
            collectList[fromIndex] = collectList[toIndex];
            collectList[toIndex] = temp;
        }

        system.UpdateInventory();
    }
}
