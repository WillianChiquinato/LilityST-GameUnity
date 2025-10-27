using UnityEngine;
using HasanSadikin.Carousel;
using UnityEngine.UI;
using DG.Tweening;

public class LocationCaroulselItem : CarouselItem<LocationData>
{
    [SerializeField] Image _image;

    protected override void OnDataUpdated(LocationData data)
    {
        base.OnDataUpdated(data);

        _image.sprite = data.sprite;
    }

    protected override void OnActivated()
    {
        base.OnActivated();

        _image.DOKill();
        _rectTransform.DOKill();

        _image.DOFade(1f, 0.25f).SetEase(Ease.Linear).SetUpdate(true);
        _rectTransform.DOScale(0.75f, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
    }

    protected override void OnDeactivated()
    {
        base.OnDeactivated();

        _image.DOKill();
        _rectTransform.DOKill();

        _image.DOFade(0.25f, 0.25f).SetEase(Ease.Linear).SetUpdate(true);
        _rectTransform.DOScale(0.5f, 0.25f).SetEase(Ease.Linear).SetUpdate(true);
    }
}
