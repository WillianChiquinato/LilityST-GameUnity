using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class Livro : MonoBehaviour
{
    public RectTransform[] pages;
    public float flipSpeed = 2f;

    private int currentPage = 0;
    public bool rotate = false;

    void Awake()
    {
        InitialState();
    }

    public void InitialState()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
        }
        pages[0].SetAsLastSibling();
    }

    public void FlipToPage(int targetPage)
    {
        if (targetPage < 0 || targetPage >= pages.Length || targetPage == currentPage)
        {
            Debug.LogWarning("já está na página alvo!");
            return;
        }

        if (rotate)
        {
            return;
        }

        if (targetPage > currentPage)
        {
            StartCoroutine(FlipPagesForward(targetPage));
        }
        else
        {
            StartCoroutine(FlipPagesBackward(targetPage));
        }
    }

    private IEnumerator FlipPagesForward(int targetPage)
    {
        while (currentPage < targetPage)
        {
            yield return StartCoroutine(FlipPage(pages[currentPage], 0, 180));
            currentPage++;
            pages[currentPage].SetAsLastSibling();
        }
    }

    private IEnumerator FlipPagesBackward(int targetPage)
    {
        while (currentPage > targetPage)
        {
            currentPage--;
            yield return StartCoroutine(FlipPage(pages[currentPage], 180, 0));
        }
    }

    private IEnumerator FlipPage(RectTransform page, float startAngle, float endAngle)
    {
        rotate = true;

        pages[currentPage].SetAsLastSibling();
        float time = 0f;
        while (time < 1f)
        {
            time += Time.unscaledDeltaTime * flipSpeed;
            float angle = Mathf.Lerp(startAngle, endAngle, time);
            page.localEulerAngles = new Vector3(0, angle, 0);
            rotate = false;
            yield return null;
        }

        page.localEulerAngles = new Vector3(0, endAngle, 0);
        rotate = false;
    }
}
