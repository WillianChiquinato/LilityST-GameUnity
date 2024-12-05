using UnityEngine;

namespace HasanSadikin.Carousel
{
    public interface ICarouselItemPositioner {
        void SetPosition(RectTransform rectTransform, int index);
        bool IsItemAfter(RectTransform a, RectTransform b);
    }
}