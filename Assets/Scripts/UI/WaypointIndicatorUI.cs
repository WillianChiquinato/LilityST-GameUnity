using UnityEngine;
using UnityEngine.UI;

public class WaypointIndicatorUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Image iconUI;
    public Image arrowUI;
    public Vector3 offsetIcon;

    private int currentFrame = 0;
    private float frameTimer = 0f;

    [Header("Arrow Settings")]
    public float arrowMargin = 50f;

    public Transform target;

    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
        if (target == null)
        {
            Debug.LogWarning("WaypointIndicatorUI: No target assigned!");
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (target == null)
        {
            // Se o alvo foi destruído, remove o indicador.
            WaypointManager.Instance.RemoveTarget(target);
            Destroy(gameObject);
            return;
        }

        UpdatePosition();
    }

    void UpdatePosition()
    {
        Vector3 screenPos = mainCamera.WorldToScreenPoint(target.position);
        bool isOffscreen = screenPos.z < 0 ||
                           screenPos.x < 0 || screenPos.x > Screen.width ||
                           screenPos.y < 0 || screenPos.y > Screen.height;

        iconUI.gameObject.SetActive(isOffscreen);
        arrowUI.gameObject.SetActive(isOffscreen);

        if (isOffscreen)
        {
            Vector3 clampedPos = screenPos;
            Vector3 clampedPosIcon = screenPos;

            clampedPos += offsetIcon;

            float halfWidth = iconUI.rectTransform.rect.width / 2;
            float halfHeight = iconUI.rectTransform.rect.height / 2;

            clampedPosIcon.x = Mathf.Clamp(clampedPosIcon.x, arrowMargin + halfWidth, Screen.width - arrowMargin - halfWidth);
            clampedPosIcon.y = Mathf.Clamp(clampedPosIcon.y, arrowMargin + halfHeight, Screen.height - arrowMargin - halfHeight);
            clampedPos.x = Mathf.Clamp(clampedPos.x, arrowMargin, Screen.width - arrowMargin);
            clampedPos.y = Mathf.Clamp(clampedPos.y, arrowMargin, Screen.height - arrowMargin);

            arrowUI.rectTransform.position = clampedPos;
            iconUI.rectTransform.position = clampedPosIcon;

            Vector3 dir = (screenPos - clampedPos).normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            arrowUI.rectTransform.rotation = Quaternion.Euler(0, 0, angle - 90f);
        }
    }
}