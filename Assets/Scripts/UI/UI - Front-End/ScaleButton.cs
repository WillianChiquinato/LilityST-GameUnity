using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScaleButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image buttonImage;
    public Sprite originalImage;
    public Sprite hoverSprite;

    void Start()
    {
        buttonImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        buttonImage.sprite = hoverSprite;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        buttonImage.sprite = originalImage;
    }
}
