using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace HasanSadikin.Carousel
{
    public class HorizontalCarouselItemPositioner : MonoBehaviour, ICarouselItemPositioner
    {
        [SerializeField] private bool _isStatic = false;
        [SerializeField] private float _duration = 0.25f;
        [SerializeField] private float _offsetX = 0f;
        [SerializeField] private float _gap = 100f;
        [SerializeField] private int _visibleItem = 3;
        [SerializeField] private Ease _ease = Ease.OutQuad;

        [Header("Debug Settings")]
        [SerializeField] private bool _debugCarouselArea = false;

        private Image _image;

        private void Awake()
        {
            _image = GetComponent<Image>();
        }

        private void Start()
        {
            // Configurações iniciais
            UpdateDebugVisuals();
            if (!_isStatic)
                UpdateSizeDelta();
        }

        private void OnValidate()
        {
            // Atualiza os visuais no editor quando algo muda
            UpdateDebugVisuals();
            if (!_isStatic)
                UpdateSizeDelta();
        }

        public void SetPosition(RectTransform rectTransform, int index)
        {
            if (_isStatic) return;

            float endValue = index * _gap + _offsetX;
            float duration = Mathf.Clamp(_duration, 0.1f, 1f);

            rectTransform.DOAnchorPosX(endValue, duration)
                .SetEase(_ease)
                .SetUpdate(true)
                .OnStart(() =>
                {
                    // Debug.Log($"Animando");
                })
                .OnComplete(() =>
                {
                    // Debug.Log($"Animando");
                });
        }

        public bool IsItemAfter(RectTransform a, RectTransform b)
        {
            return a.anchoredPosition.x > b.anchoredPosition.x;
        }

        private void UpdateSizeDelta()
        {
            if (_image == null || _image.rectTransform == null) return;

            Vector2 newSize = new Vector2(_visibleItem * _gap, _image.rectTransform.sizeDelta.y);

            if (_image.rectTransform.sizeDelta != newSize)
            {
                _image.rectTransform.sizeDelta = newSize;

#if UNITY_EDITOR
                UnityEditor.Undo.RecordObject(_image.rectTransform, "Update RectTransform SizeDelta");
                UnityEditor.EditorUtility.SetDirty(_image.rectTransform);
#endif
            }
        }

        private void UpdateDebugVisuals()
        {
            if (_image == null) return;

            // Define uma cor transparente para depuração
            _image.color = _debugCarouselArea
                ? new Color(1, 1, 1, 0.2f)
                : new Color(1, 1, 1, 0.004f);
        }
    }
}
