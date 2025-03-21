using System.Collections.Generic;
using UnityEngine;

public class FilhoteDetectItem : MonoBehaviour
{
    public FilhoteDragão FilhoteDragão;

    void Awake()
    {
        FilhoteDragão = GetComponentInParent<FilhoteDragão>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemArremessar"))
        {
            FilhoteDragão.targetLility = true;
            FilhoteDragão.targetObjects = collision.gameObject;
        }
    }
}
