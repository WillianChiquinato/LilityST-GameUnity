using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionCheck : MonoBehaviour
{
    public Image imageSelect;
    public TextMeshProUGUI textoSelect;

    public Item_SlotUI[] slotsUI;
    public int selectedIndex = -1;

    [System.Obsolete]
    void Awake()
    {
        slotsUI = GameObject.FindObjectsOfType<Item_SlotUI>().ToArray();
        textoSelect.text = "";
    }

    void Start()
    {
        for (int i = 0; i < slotsUI.Length; i++)
        {
            int index = i;
            slotsUI[i].OnSlotClicked += OnSlotSelected;
        }
    }

    public void OnSlotSelected(int slotIndex)
    {
        selectedIndex = slotIndex;
        ReceberSelect();
    }

    public void ReceberSelect()
    {
        if (selectedIndex >= 0 && selectedIndex < slotsUI.Length)
        {
            imageSelect.sprite = slotsUI[selectedIndex].item.itemData.ilustracao;
            textoSelect.text = slotsUI[selectedIndex].item.itemData.Description;
        }
    }
}
