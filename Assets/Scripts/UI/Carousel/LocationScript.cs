using UnityEngine;
using HasanSadikin.Carousel;

[System.Serializable]
public class LocationData
{
    public Sprite sprite;
    public int itemIndex;
    public GameObject objectToActivate;
}

public class LocationScript : CarouselController<LocationData>
{
    public UI uI;

    private void OnEnable()
    {
        OnItemSelected.AddListener(LogItem);
        OnCurrentItemUpdated.AddListener(LogItem);
    }

    private void LogItem(LocationData data)
    {
        switch (data.itemIndex)
        {
            case 0:
                uI.FlipToPage(0);
                Debug.Log("Item 0 clicado");
                break;
            case 1:
                uI.FlipToPage(1);
                Debug.Log("Item 1 clicado");
                break;
            case 2:
                uI.FlipToPage(0);
                Debug.Log("Item 2 clicado");
                break;
            case 3:
                uI.FlipToPage(1);
                Debug.Log("Item 3 clicado");
                break;
            case 4:
                uI.FlipToPage(0);
                Debug.Log("Item 4 clicado");
                break;
            default:
                Debug.Log("Item desconhecido clicado");
                break;
        }
    }
}
