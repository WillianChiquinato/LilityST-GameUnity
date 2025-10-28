using UnityEngine;
using HasanSadikin.Carousel;

[DefaultExecutionOrder(100)]
[System.Serializable]
public class LocationData
{
    public Sprite sprite;
    public int itemIndex;
}

[DefaultExecutionOrder(100)]
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
                break;
            case 1:
                uI.FlipToPage(1);
                break;
            case 2:
                uI.FlipToPage(2);
                break;
            default:
                Debug.Log("Item desconhecido clicado");
                break;
        }
    }
}
