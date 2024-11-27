using UnityEngine;
using HasanSadikin.Carousel;

[System.Serializable]
public class LocationData
{
    public Sprite sprite;
}

public class LocationScript : CarouselController<LocationData>
{
    private void OnEnable()
    {
        OnItemSelected.AddListener(LogItem);
        OnCurrentItemUpdated.AddListener(LogItem);
    }

    private void LogItem(LocationData data)
    {
        //Aqui fará a mudança de tela e tals, reconhecimento aqui
        Debug.Log(data.sprite);
    }
}
