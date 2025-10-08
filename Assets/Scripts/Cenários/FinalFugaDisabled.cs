using UnityEngine;

public class FinalFugaDisabled : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.TriggerNoUseArgument(new string[] { "Dash", "Run" });
        }
    }
}
