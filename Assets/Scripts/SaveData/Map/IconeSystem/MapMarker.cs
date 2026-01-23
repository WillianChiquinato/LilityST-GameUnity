using UnityEngine;

public class MapMarker : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
    [TextArea]
    public string note;

    public void SetNote(string text)
    {
        note = text;
    }

    public void SetIcon(MapIconData data)
    {
        spriteRenderer.sprite = data.icon;
    }
}
