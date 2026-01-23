using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MapToggle : MonoBehaviour
{
    public Camera mapCamera;
    public CanvasGroup mapUI;
    public MapToggleMode mapToggleMode;

    void Start()
    {
        StartCoroutine(WaitToLoading());
    }

    IEnumerator WaitToLoading()
    {
        yield return new WaitForSeconds(0.75f);

        mapCamera = FindFirstObjectByType<MapCameraZoom>().mapCamera;
        CanvasGroup ui = mapCamera
            .transform
            .parent
            .GetComponentInChildren<CanvasGroup>(true);

        mapUI = ui;

        mapCamera.enabled = false;
        mapUI.alpha = 0;
        mapUI.interactable = false;
        mapUI.blocksRaycasts = false;

        mapToggleMode = FindFirstObjectByType<MapToggleMode>();
    }

    void LateUpdate()
    {
        if (GameManager.instance != null && mapCamera != null)
        {
            GameManager.instance.player.isMapOpened = mapCamera.enabled;
        }
    }

    public void OnMap(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!mapCamera.enabled)
            {
                GameManager.instance.player.OpenMapBool = true;
                Invoke("OpenMapAnim", 1.4f);
                return;
            }

            OpenMapAnim();
        }
    }

    public void OpenMapAnim()
    {
        GameManager.instance.player.canMove = true;
        GameManager.instance.player.OpenMapBool = false;

        mapCamera.enabled = !mapCamera.enabled;
        if (!mapCamera.enabled)
        {
            StartCoroutine(GameManager.instance.FadeInCanvasGroup(GameManager.instance.GUI.GetComponent<CanvasGroup>(), 0.4f));
            StartCoroutine(GameManager.instance.FadeOutCanvasGroup(mapUI, 0.4f));

            mapToggleMode.freeMoveEnabled = false;
        }
        else
        {
            StartCoroutine(GameManager.instance.FadeOutCanvasGroup(GameManager.instance.GUI.GetComponent<CanvasGroup>(), 0.4f));
            StartCoroutine(GameManager.instance.FadeInCanvasGroup(mapUI, 0.4f));
        }
    }
}