using UnityEngine;

public class StopSoldierPlayerDetect : MonoBehaviour
{
    public bool isPlaying = false;

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlaying = true;
        }
    }
}
