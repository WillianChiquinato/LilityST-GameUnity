using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum ToastType
{
    Success,
    Error,
    Alert
}

public class Toast : MonoBehaviour
{
    [Header("Componentes")]
    public TextMeshProUGUI messageText;
    public Image background;
    public Image icon;
    public CanvasGroup canvasGroup;

    [Header("Materiais")]
    public Color successColor = Color.green;
    public Color errorColor = Color.red;
    public Color alertColor = Color.yellow;

    public Sprite successIcon;
    public Sprite errorIcon;
    public Sprite alertIcon;

    [Header("Configurações")]
    public float duration = 2f;
    public float fadeDuration = 0.3f;
    public float floatDistance = 50f;

    public RectTransform rectTransform;
    public Vector2 startPos;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPos = rectTransform.anchoredPosition;
    }

    void Update()
    {
        // Atualiza a posição do toast
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += Vector2.up * 10f * Time.unscaledDeltaTime;
        }
    }

    public void Show(string message, ToastType type)
    {
        Debug.Log("Toast chamado!");
        messageText.text = message;

        switch (type)
        {
            case ToastType.Success:
                background.color = successColor;
                messageText.color = successColor;
                messageText.fontStyle = FontStyles.Bold;
                icon.sprite = successIcon;
                break;
            case ToastType.Error:
                background.color = errorColor;
                messageText.color = errorColor;
                messageText.fontStyle = FontStyles.Bold;
                icon.sprite = errorIcon;
                break;
            case ToastType.Alert:
                background.color = alertColor;
                messageText.color = alertColor;
                messageText.fontStyle = FontStyles.Bold;
                icon.sprite = alertIcon;
                break;
        }

        gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(AnimateToast());
    }

    private IEnumerator AnimateToast()
    {
        this.gameObject.SetActive(true);
        Debug.Log("Toast chamado ANIMAÇÃO!");
        // reset posição e alpha
        rectTransform.anchoredPosition = startPos;
        canvasGroup.alpha = 0;

        // Fade in + movimento para cima
        float t = 0;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / fadeDuration;

            canvasGroup.alpha = progress;

            yield return null;
        }
        canvasGroup.alpha = 1;

        // Espera tempo ativo
        yield return new WaitForSecondsRealtime(duration);

        t = 0;
        Vector2 midPos = rectTransform.anchoredPosition;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = t / fadeDuration;

            canvasGroup.alpha = 1 - progress;

            yield return null;
        }

        canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }
}
