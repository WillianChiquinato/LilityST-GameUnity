using UnityEngine;

public class checkpointQuestsTeste : MonoBehaviour
{
    public CheckPointQuest teste;

    void Update()
    {
        if (teste != null)
        {
            Debug.Log("CheckPointQuest encontrado!");
        }
        else
        {
            Debug.Log("CheckPointQuest não encontrado!");
            teste = GameObject.FindFirstObjectByType<CheckPointQuest>();
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            teste.CheckPointCollected();
            Destroy(gameObject, 1f);
        }
    }
}
