using UnityEngine;

public class LagartoItemDetect : MonoBehaviour
{
    public LagartoPatrulha lagartoPatrulha;

    void Awake()
    {
        lagartoPatrulha = GetComponentInParent<LagartoPatrulha>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemArremessar"))
        {
            lagartoPatrulha.targetLility = true;
            lagartoPatrulha.targetLilityObject = collision.gameObject;
        }
    }
}
