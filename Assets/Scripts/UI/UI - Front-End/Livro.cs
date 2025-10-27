using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Livro : MonoBehaviour
{
    public RectTransform[] pages;
    public Vector3[] pagesInitialRotation;
    public Button[] pageButtons;
    public float flipSpeed = 2f;
    public float scaleSpeed = 6f;

    private int currentPage = 0;
    public bool rotate = false;
    private Coroutine scaleRoutine;

    void Awake()
    {
        InitialState();

        for (int i = 0; i < pageButtons.Length; i++)
        {
            int index = i;
            pageButtons[i].onClick.AddListener(() => OnPageButtonClick(index));
        }
    }

    public void InitialState()
    {
        currentPage = 1;
        pages[0].SetAsLastSibling();
        pages[0].localEulerAngles = pagesInitialRotation[0];

        // Botões
        for (int i = 0; i < pageButtons.Length; i++)
        {
            pageButtons[i].transform.localScale = Vector3.one;

            MouseEnterCaderno mouseEnterCaderno = pageButtons[i].GetComponent<MouseEnterCaderno>();
            if (mouseEnterCaderno != null)
            {
                mouseEnterCaderno.isSelected = (i == 0);
                if (i == 0)
                {
                    pageButtons[i].transform.localScale = mouseEnterCaderno.targetScale;
                    pageButtons[i].transform.localPosition = pageButtons[i].transform.localPosition + mouseEnterCaderno.hoverOffset;
                }
            }
        }

        // Páginas restantes
        for (int i = 1; i < pages.Length; i++)
        {
            pages[i].transform.rotation = Quaternion.identity;
            if (i < pagesInitialRotation.Length)
            {
                pages[i].localEulerAngles = pagesInitialRotation[i];
            }
        }

        pages[0].SetAsLastSibling();
    }

    private void OnPageButtonClick(int buttonIndex)
    {
        if (rotate) return;

        int targetPage = buttonIndex + 1;
        if (targetPage == currentPage) return;

        FlipToPage(targetPage);
        AnimateButtonScale(buttonIndex);

        for (int i = 0; i < pageButtons.Length; i++)
        {
            MouseEnterCaderno mouseEnterCaderno = pageButtons[i].GetComponent<MouseEnterCaderno>();
            if (mouseEnterCaderno != null)
            {
                mouseEnterCaderno.isSelected = (i == buttonIndex);
            }
        }
    }

    private void AnimateButtonScale(int selectedIndex)
    {
        // Para qualquer animação anterior
        if (scaleRoutine != null)
            StopCoroutine(scaleRoutine);

        scaleRoutine = StartCoroutine(ScaleButtonsRoutine(selectedIndex));
    }

    private IEnumerator ScaleButtonsRoutine(int selectedIndex)
    {
        float duration = 0.3f;

        for (int i = 0; i < pageButtons.Length; i++)
        {
            Transform btn = pageButtons[i].transform;
            MouseEnterCaderno mouseEnterCaderno = btn.GetComponent<MouseEnterCaderno>();
            Vector3 targetScale = (i == selectedIndex) ? mouseEnterCaderno.targetScale : Vector3.one;

            // Suave
            StartCoroutine(SmoothScale(btn, targetScale, duration));
        }

        yield return null;
    }

    private IEnumerator SmoothScale(Transform target, Vector3 targetScale, float duration)
    {
        Vector3 startScale = target.localScale;
        float time = 0f;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime * scaleSpeed;
            target.localScale = Vector3.Lerp(startScale, targetScale, time / duration);
            yield return null;
        }

        target.localScale = targetScale;
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

            UpdatePageContentVisibility(page, angle <= 90);

            yield return null;
        }

        page.localEulerAngles = new Vector3(0, endAngle, 0);
        UpdatePageContentVisibility(page, endAngle == 0);
        rotate = false;
    }

    private void UpdatePageContentVisibility(RectTransform page, bool showFront)
    {
        Transform frontContent = page.Find("FrontContent");
        Transform backContent = page.Find("BackContent");

        if (frontContent != null)
        {
            frontContent.gameObject.SetActive(showFront);

            Canvas frontCanvas = frontContent.GetComponent<Canvas>();
            if (frontCanvas != null)
            {
                frontCanvas.overrideSorting = showFront;
                frontCanvas.sortingOrder = showFront ? 1 : -1;

                frontContent.GetComponent<GraphicRaycaster>().enabled = showFront;
            }
        }

        if (backContent != null)
        {
            backContent.gameObject.SetActive(!showFront);

            Canvas backCanvas = backContent.GetComponent<Canvas>();
            if (backCanvas != null)
            {
                backCanvas.overrideSorting = !showFront;
                backCanvas.sortingOrder = !showFront ? 1 : -1;

                backContent.GetComponent<GraphicRaycaster>().enabled = !showFront;
            }
        }
    }
}
