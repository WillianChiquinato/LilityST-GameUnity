using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class skillTreeCheck : MonoBehaviour
{
    public Image imageSelect;
    public TextMeshProUGUI textoSelect;
    public Button botaoSelect;

    public SkillTreeUI[] skillTreeUI;
    private int selectedIndex = -1;

    [System.Obsolete]
    void Awake()
    {
        skillTreeUI = GameObject.FindObjectsOfType<SkillTreeUI>().OrderBy(skill => skill.name).ToArray();
        textoSelect.text = "";
    }

    public void OnSlotSelected(int slotIndex)
    {
        selectedIndex = slotIndex;
        receberSelect();
    }

    public void receberSelect()
    {
        if (selectedIndex >= 0 && selectedIndex < skillTreeUI.Length)
        {
            imageSelect.sprite = ConvertTextureToSprite(skillTreeUI[selectedIndex].imagemRef);
            textoSelect.text = skillTreeUI[selectedIndex].textoRef;
            botaoSelect.onClick.RemoveAllListeners();
            botaoSelect.onClick.AddListener(skillTreeUI[selectedIndex].UnlockSkillSlot);
        }
    }

    private Sprite ConvertTextureToSprite(Texture texture)
    {
        if (texture is Texture2D texture2D)
        {
            return Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
        }

        return null;
    }
}
