using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipItems : MonoBehaviour
{
    public static ToolTipItems Instance;
    public Vector2 offset;
    public GameObject toolTipItem;
    public Image itemIcon;
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemDescription;
    public TextMeshProUGUI itemStack;

    void Awake()
    {
        Instance = this;
        toolTipItem.SetActive(false);

        //Desliga o raycast para nao bugar a informação.
        foreach (var graphic in toolTipItem.GetComponentsInChildren<Graphic>())
        {
            graphic.raycastTarget = false;
        }
    }

    void Update()
    {
        if (toolTipItem.activeSelf)
        {
            toolTipItem.transform.position = (Vector2)Input.mousePosition + offset;
        }
    }

    public void ShowToolTip(ItemData itemData, int stackSize)
    {
        if (itemData != null)
        {
            itemIcon.sprite = itemData.Icon;
            itemName.text = itemData.ItemName;
            itemDescription.text = itemData.Description;
            itemStack.text = "x" + stackSize.ToString();
            toolTipItem.SetActive(true);
        }
    }

    public void ShowToolTipFragmento(FragmentoData fragmentoData, int stackSize)
    {
        if (fragmentoData != null)
        {
            itemIcon.sprite = fragmentoData.Icon;
            itemName.text = fragmentoData.NomeFragmento;
            itemDescription.text = fragmentoData.DescricaoFragmento;
            itemStack.text = "x" + stackSize.ToString();
            toolTipItem.SetActive(true);
        }
    }

    public void HideToolTip()
    {
        toolTipItem.SetActive(false);
    }
}
