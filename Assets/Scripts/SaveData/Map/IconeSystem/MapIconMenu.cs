using UnityEngine;

public class MapIconMenu : MonoBehaviour
{
    public static MapIconMenu Instance;

    public MapIconData selectedIcon;

    void Awake()
    {
        Instance = this;
    }

    public void SelectIcon(MapIconData icon)
    {
        selectedIcon = icon;
    }
}
