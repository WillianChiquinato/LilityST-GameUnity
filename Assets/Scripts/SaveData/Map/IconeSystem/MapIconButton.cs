using UnityEngine;
using UnityEngine.UI;

public class MapIconButton : MonoBehaviour
{
    public MapIconData iconData;
    public Image iconImage;

    void Start()
    {
        iconImage.sprite = iconData.icon;
    }

    public void OnClick()
    {
        MapIconMenu.Instance.SelectIcon(iconData);
        MapDragGhost.Instance.Show(iconData.icon);
    }
}
