using UnityEngine;

public class MapPlacement : MonoBehaviour
{
    public Camera mapCamera;
    public GameObject markerPrefab;

    private GameObject currentMarker;
    public Transform mapRootIcons;

    void Update()
    {
        if (!Input.GetMouseButtonDown(0))
            return;

        if (MapIconMenu.Instance.selectedIcon == null)
            return;

        Vector2 mousePos = Input.mousePosition;

        // converte para viewport da camera do mapa
        Vector3 viewportPos = mapCamera.ScreenToViewportPoint(mousePos);

        // só aceita clique dentro da camera do mapa
        if (viewportPos.x < 0 || viewportPos.x > 1 ||
            viewportPos.y < 0 || viewportPos.y > 1)
            return;

        // agora sim converte para mundo do mapa
        Vector3 worldPos = mapCamera.ViewportToWorldPoint(
            new Vector3(viewportPos.x, viewportPos.y, mapCamera.nearClipPlane)
        );

        worldPos.z = 0;

        GameObject marker = Instantiate(markerPrefab, worldPos, Quaternion.identity, mapRootIcons);

        var markerScript = marker.GetComponent<MapMarker>();
        markerScript.SetIcon(MapIconMenu.Instance.selectedIcon);

        MapUI.Instance.OpenNoteInput(markerScript);

        MapDragGhost.Instance.Hide();
        MapIconMenu.Instance.selectedIcon = null;
    }
}
