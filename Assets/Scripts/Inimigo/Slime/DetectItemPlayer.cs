using UnityEngine;

public class DetectItemPlayer : MonoBehaviour
{
    public SlimeMoviment slimeMoviment;

    void Awake()
    {
        slimeMoviment = GetComponentInParent<SlimeMoviment>();
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("ItemArremessar"))
        {
            slimeMoviment.targetLility = true;
            slimeMoviment.targetLilityObject = collision.gameObject;
        }
    }
}
