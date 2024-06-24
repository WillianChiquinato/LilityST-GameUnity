using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Camera CameraMain;

    [SerializeField]
    private UIinvetárioItem item;

    public void Awake()
    {
        //Root, raiz, objeto tirado da raiz
        canvas = transform.root.GetComponent<Canvas>();
        CameraMain = Camera.main;
        item = GetComponentInChildren<UIinvetárioItem>();
    }

    public void SetData(Sprite sprite, int quantidade)
    {
        item.SetData(sprite, quantidade);
    }

    void Update()
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)canvas.transform, Input.mousePosition, canvas.worldCamera, out position);
        transform.position = canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool valor)
    {
        Debug.Log($"Peguei o Item {valor}");
        gameObject.SetActive(valor);
    }
}
