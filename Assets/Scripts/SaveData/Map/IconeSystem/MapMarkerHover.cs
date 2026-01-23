using System.Collections;
using UnityEngine;

public class MapMarkerHover : MonoBehaviour
{
    private MapMarker marker;

    private Coroutine scaleCoroutine;
    private Vector3 baseScale;

    void Start()
    {
        marker = GetComponent<MapMarker>();
        baseScale = transform.localScale;
    }

    void OnMouseEnter()
    {
        ToolTipUI.Instance.Show(marker.note);
        StartScale(baseScale * 1.5f);
    }

    void OnMouseExit()
    {
        ToolTipUI.Instance.Hide();
        StartScale(baseScale);
    }

    void StartScale(Vector3 targetScale)
    {
        if (scaleCoroutine != null)
            StopCoroutine(scaleCoroutine);

        scaleCoroutine = StartCoroutine(ScaleCoroutine(targetScale));
    }

    IEnumerator ScaleCoroutine(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float duration = 0.2f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localScale = targetScale;
        scaleCoroutine = null;
    }
}
