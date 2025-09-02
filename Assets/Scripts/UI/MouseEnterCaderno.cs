using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEnterCaderno : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 targetScale = new Vector3(1.2f, 1.2f, 1f);
    public Vector3 positionOffset = Vector3.zero;
    public float Speed = 5f;

    private Vector3 originalScale;
    public bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        Vector3 targetS = isHovered ? targetScale : originalScale;

        transform.localScale = Vector3.Lerp(transform.localScale, targetS, Time.unscaledDeltaTime * Speed);
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
