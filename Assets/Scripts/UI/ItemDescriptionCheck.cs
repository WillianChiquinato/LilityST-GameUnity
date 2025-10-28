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
        switch (tipo)
        {
            case SlotType.Comum:
                AtualizarDescricao(0, slotsComunsUI, slotIndex);
                break;
            case SlotType.Coletavel:
                AtualizarDescricao(1, slotsColetaveisUI, slotIndex);
                break;
            case SlotType.Quest:
                AtualizarDescricao(2, slotsQuestUI, slotIndex);
                break;
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
