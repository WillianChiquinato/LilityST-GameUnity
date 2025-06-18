using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseEnterCaderno : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 targetScale = new Vector3(0f, 0f, 0f);
    public float scaleSpeed = 5f;

    private Vector3 originalScale;
    public bool isHovered = false;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (isHovered)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.unscaledDeltaTime * scaleSpeed);
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.unscaledDeltaTime * scaleSpeed);
        }
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
