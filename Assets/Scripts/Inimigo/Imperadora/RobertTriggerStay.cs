using UnityEngine;

public class RobertTriggerStay : MonoBehaviour
{
    public bool isPlayerStaying;
    public RobertMoviment robertMoviment;

    void Awake()
    {
        robertMoviment = GameObject.FindFirstObjectByType<RobertMoviment>();
        isPlayerStaying = false;
    }

    void LateUpdate()
    {
        transform.position = robertMoviment.transform.position + new Vector3(20f, 0f, 0f);
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isPlayerStaying)
        {
            isPlayerStaying = true;
            robertMoviment.CanMove = false;
            robertMoviment.lilithChecked = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerStaying = false;
            robertMoviment.CanMove = true;
        }
    }
}
