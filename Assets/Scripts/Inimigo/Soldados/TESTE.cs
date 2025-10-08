using System.Collections;
using UnityEngine;

public class TESTE : MonoBehaviour
{
    public bool spawnSoldiers = false;
    public GameObject soldierPrefab;

    void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }

    IEnumerator WaitAndSpawn()
    {
        yield return new WaitForSeconds(2f);
        spawnSoldiers = true;
    }

    void Update()
    {
        if (spawnSoldiers)
        {
            Instantiate(soldierPrefab, transform.position, Quaternion.identity);
            Instantiate(soldierPrefab, transform.position, Quaternion.identity);
            spawnSoldiers = false;
        }
    }
}
