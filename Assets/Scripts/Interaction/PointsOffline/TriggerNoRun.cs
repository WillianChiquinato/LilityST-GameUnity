using UnityEngine;

public class TriggerNoRun : MonoBehaviour
{
    public PlayerMoviment playerMovement;
    public GameObject[] interfacesMoment;

    void Start()
    {
        playerMovement = GameObject.FindFirstObjectByType<PlayerMoviment>();
        foreach (GameObject interfaceMoment in interfacesMoment)
        {
            interfaceMoment.SetActive(false);
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.RunTiming = 0f;
            playerMovement.IsRunning = false;
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.IsRunning = true;
        }
    }

}
