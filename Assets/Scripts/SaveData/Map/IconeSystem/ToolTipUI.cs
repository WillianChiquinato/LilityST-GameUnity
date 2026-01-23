using TMPro;
using UnityEngine;

public class ToolTipUI : MonoBehaviour
{
    public static ToolTipUI Instance;

    public GameObject tooltip;
    public TMP_Text tooltipText;

    public Vector2 offset = new Vector2(15, -15);

    void Awake()
    {
        Instance = this;
        tooltip.SetActive(false);
    }

    void Update()
    {
        if (tooltip.activeSelf)
        {
            tooltip.transform.position = (Vector2)Input.mousePosition + offset;
        }
    }

    public void Show(string text)
    {
        if (string.IsNullOrEmpty(text)) return;

        tooltipText.text = text;
        tooltip.SetActive(true);
    }

    public void Hide()
    {
        tooltip.SetActive(false);
    }
}
