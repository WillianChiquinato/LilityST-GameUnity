using UnityEngine;

public class TriggerNoRun : MonoBehaviour
{
    public GameObject[] interfacesMoment;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.IsRunning = false;
            foreach (GameObject interfaceMoment in interfacesMoment)
            {
                if (interfaceMoment.GetComponent<CanvasGroup>())
                {
                    StartCoroutine(GameManager.instance.FadeOutCanvasGroup(interfaceMoment.GetComponent<CanvasGroup>(), 0.5f));
                    return;
                }
                interfaceMoment.SetActive(true);
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.RunTiming = 0f;
            GameManager.instance.player.IsRunning = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.player.IsRunning = true;
        }
    }

}
