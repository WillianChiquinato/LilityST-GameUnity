using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class fadeUI : MonoBehaviour
{
    CanvasGroup canvasGroup;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeUIOut(float segundos)
    {
        StartCoroutine(FadeOut(segundos));
    }
    public void FadeUIIn(float segundos)
    {
        StartCoroutine(FadeIn(segundos));
    }

    IEnumerator FadeIn(float segundos)
    {
        canvasGroup.alpha = 0;

        while (canvasGroup.alpha < 1)
        {
            canvasGroup.alpha += Time.unscaledDeltaTime / segundos;
            yield return null;
        }
        
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        yield return null;
    }

    IEnumerator FadeOut(float segundos)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 1;
        while (canvasGroup.alpha > 0)
        {
            canvasGroup.alpha -= Time.unscaledDeltaTime / segundos;
            yield return null;
        }

        yield return null;
    }
}
