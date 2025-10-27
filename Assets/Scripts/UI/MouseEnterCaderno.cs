using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEnterCaderno : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 hoverOffset = new Vector3(-10f, 0f, 0f);
    public float Speed = 5f;

    private Vector3 originalScale;
    public Vector3 originalPosition;
    public bool isHovered = false;

    public bool isSelected = false;

    void Awake()
    {
        originalScale = Vector3.one;
        originalPosition = transform.localPosition;
    }

    void Update()
    {
        if (isSelected)
        {
            return;
        }

        Vector3 targetS = isHovered ? targetScale : originalScale;
        Vector3 targetP = isHovered ? originalPosition + hoverOffset : originalPosition;

        transform.localScale = Vector3.Lerp(transform.localScale, targetS, Time.unscaledDeltaTime * Speed);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetP, Time.unscaledDeltaTime * Speed);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
    }
}
