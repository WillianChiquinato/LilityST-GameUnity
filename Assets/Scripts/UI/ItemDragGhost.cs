using UnityEngine;
using UnityEngine.UI;

public class ItemDragGhost : MonoBehaviour
{
    public static ItemDragGhost Instance;

    [SerializeField] private Image ghostImage;
    private CanvasGroup canvasGroup;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        Hide();
    }

    public void Show(Sprite sprite, Vector2 position)
    {
        ghostImage.sprite = sprite;
        transform.position = position;
        canvasGroup.alpha = 0.8f;
        ghostImage.enabled = true;
    }

    public void Move(Vector2 position)
    {
        transform.position = position;
    }

    public void Hide()
    {
        ghostImage.enabled = false;
        canvasGroup.alpha = 0f;
    }
}
