using UnityEngine;

public class vidroScript : MonoBehaviour
{
    public GameObject prefabEnemy;
    public GameObject spawnSoldado;

    private void OnTriggerEnter2D(Collider2D colisor)
    {
        if (colisor.gameObject.CompareTag("Player"))
        {
            Debug.Log("Inimigo SPAWN");
            Instantiate(prefabEnemy, spawnSoldado.transform.position, Quaternion.identity);
        }
    }
}
