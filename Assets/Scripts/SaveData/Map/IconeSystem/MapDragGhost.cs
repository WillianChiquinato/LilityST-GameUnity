using UnityEngine;
using UnityEngine.UI;

public class MapDragGhost : MonoBehaviour
{
    public static MapDragGhost Instance;

    public Image ghostImage;
    public Vector2 offset = new Vector2(20, -20);

    void Awake()
    {
        Instance = this;
        gameObject.SetActive(false);
    }

    void Update()
    {
        transform.position = (Vector2)Input.mousePosition + offset;
    }

    public void Show(Sprite icon)
    {
        ghostImage.sprite = icon;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
