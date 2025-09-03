using System.Collections;
using UnityEngine;

public class fadeUI : MonoBehaviour
{
    public static fadeUI Instance { get; private set; }
    public LevelTransicao levelTransicao;

    public CanvasGroup canvasGroupOptions;
    public CanvasGroup canvasGroupMultipleSaves;
    public CanvasGroup canvasGroupMainMenu;

    private CanvasGroup currentGroup;

    void Awake()
    {
        levelTransicao = FindFirstObjectByType<LevelTransicao>();
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        SetCanvasGroup(canvasGroupMainMenu, true);
        SetCanvasGroup(canvasGroupOptions, false);
        SetCanvasGroup(canvasGroupMultipleSaves, false);

        currentGroup = canvasGroupMainMenu;
    }

    public void ShowMenu(int index)
    {
        // Desliga todos primeiro
        SetCanvasGroup(canvasGroupOptions, false);
        SetCanvasGroup(canvasGroupMultipleSaves, false);
        SetCanvasGroup(canvasGroupMainMenu, false);

        // Liga o alvo com fade
        CanvasGroup target = GetCanvasGroupByIndex(index);
        StartCoroutine(FadeIn(target));

        currentGroup = target;
    }

    private CanvasGroup GetCanvasGroupByIndex(int index)
    {
        switch (index)
        {
            case 1: return canvasGroupOptions;
            case 2: return canvasGroupMultipleSaves;
            default: return canvasGroupMainMenu;
        }
    }

    private void SetCanvasGroup(CanvasGroup group, bool active)
    {
        if (active)
        {
            group.alpha = 1f;
            group.interactable = true;
            group.blocksRaycasts = true;
            group.gameObject.SetActive(true);
        }
        else
        {
            StartCoroutine(FadeOut(group));
        }
    }

    private IEnumerator FadeIn(CanvasGroup group)
    {
        group.gameObject.SetActive(true);
        group.alpha = 0f;

        while (group.alpha < 1f)
        {
            group.alpha += Time.unscaledDeltaTime / 0.5f;
            yield return null;
        }

        group.alpha = 1f;
        group.interactable = true;
        group.blocksRaycasts = true;
    }

    private IEnumerator FadeOut(CanvasGroup group)
    {
        group.interactable = false;
        group.blocksRaycasts = false;

        while (group.alpha > 0f)
        {
            group.alpha -= Time.unscaledDeltaTime / 0.5f;
            yield return null;
        }

        group.alpha = 0f;
        group.gameObject.SetActive(false);
    }
}
