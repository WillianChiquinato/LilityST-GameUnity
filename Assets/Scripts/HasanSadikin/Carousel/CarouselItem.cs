using System;
using UnityEngine;
using UnityEngine.EventSystems;



namespace HasanSadikin.Carousel
{
    public abstract class CarouselItem<T> : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private T _data;

        public event Action<CarouselItem<T>> OnSelected;

        public RectTransform _rectTransform;
        public GameObject objetoAtivado;
        public T Data
        {
            get => _data;
            set
            {
                _data = value;
                OnDataUpdated(_data);
            }
        }

        protected virtual void OnValidate()
        {
            if (_rectTransform == null) _rectTransform = GetComponent<RectTransform>();
        }

        protected virtual void OnDataUpdated(T data) { }

        internal virtual void SetActive(bool value)
        {
            if (value) OnActivated();
            else OnDeactivated();
        }

        protected virtual void OnActivated()
        {
            if (objetoAtivado != null)
            {
                objetoAtivado.SetActive(true);
            }
        }
        protected virtual void OnDeactivated()
        {
            if (objetoAtivado != null)
            {
                objetoAtivado.SetActive(false);
            }
        }
        public virtual void OnPointerClick(PointerEventData eventData)
        {
            OnSelected?.Invoke(this);
        }
    }
}