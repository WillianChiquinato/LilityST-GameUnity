using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionCheck : MonoBehaviour
{
    public Image[] imageSelects;
    public TextMeshProUGUI[] textoSelects;

    private Item_SlotUI[] slotsColetaveisUI;
    private Item_SlotUI[] slotsComunsUI;
    private Item_SlotUI[] slotsQuestUI;

    public Sprite selectedSprite;
    public Sprite defaultSprite;

    private Item_SlotUI currentSelectedSlot = null;
    public int selectedIndex = -1;

    void Awake()
    {
        for (int i = 0; i < imageSelects.Length; i++)
        {
            imageSelects[i].sprite = null;
            imageSelects[i].color = new Color(1, 1, 1, 0);
            textoSelects[i].text = "";
        }
    }

    void Start()
    {
        var inv = inventory_System.instance;
        slotsColetaveisUI = inv.coletaveisItemSlot.ToArray();
        slotsComunsUI = inv.inventoryItemSlot.ToArray();
        slotsQuestUI = inv.documentosItemSlot.ToArray();

        foreach (var slot in slotsComunsUI)
        {
            slot.OnSlotClicked += OnSlotSelected;
        }

        foreach (var slot in slotsColetaveisUI)
        {
            slot.OnSlotClicked += OnSlotSelected;
        }

        foreach (var slot in slotsQuestUI)
        {
            slot.OnSlotClicked += OnSlotSelected;
        }
    }

    public void OnSlotSelected(SlotType tipo, int slotIndex)
    {
        Item_SlotUI[] slots = null;
        int paginaIndex = 0;

        switch (tipo)
        {
            case SlotType.Comum:
                slots = slotsComunsUI;
                paginaIndex = 0;
                break;
            case SlotType.Coletavel:
                slots = slotsColetaveisUI;
                paginaIndex = 1;
                break;
            case SlotType.Quest:
                slots = slotsQuestUI;
                paginaIndex = 2;
                break;
        }

        if (slots != null && slotIndex >= 0 && slotIndex < slots.Length)
        {
            // Desselecionar o slot anterior
            if (currentSelectedSlot != null)
            {
                currentSelectedSlot.SetSelected(false);
                currentSelectedSlot.GetComponent<Image>().sprite = defaultSprite;
            }

            // Selecionar o novo slot
            currentSelectedSlot = slots[slotIndex];
            currentSelectedSlot.SetSelected(true);
            currentSelectedSlot.GetComponent<Image>().sprite = selectedSprite;

            AtualizarDescricao(paginaIndex, slots, slotIndex);
        }
    }

    private void AtualizarDescricao(int paginaIndex, Item_SlotUI[] slots, int index)
    {
        if (index < 0 || index >= slots.Length) return;

        ItemData itemData = slots[index].item?.itemData;
        if (itemData != null)
        {
            imageSelects[paginaIndex].sprite = itemData.ilustracao;
            imageSelects[paginaIndex].color = Color.white;
            textoSelects[paginaIndex].text = itemData.Description;
        }
        else
        {
            imageSelects[paginaIndex].sprite = null;
            imageSelects[paginaIndex].color = new Color(1, 1, 1, 0);
            textoSelects[paginaIndex].text = "";
        }
    }
}
