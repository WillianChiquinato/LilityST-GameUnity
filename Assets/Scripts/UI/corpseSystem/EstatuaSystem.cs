using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

public class EstatuaSystem : MonoBehaviour
{
    public static EstatuaSystem Instance;

    private List<EstatuaDrop> activeEstatuas = new List<EstatuaDrop>();
    private List<EstatuaData> estatuaDataList = new List<EstatuaData>();
    private int estatuaLimit = 3;

    [SerializeField] private GameObject estatuaPrefab;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RecreateEstatuas();
    }

    public void CreateEstatua(Vector3 position, int xpAmount, List<ItemData> droppedItems)
    {
        GameObject estatuaObj = Instantiate(estatuaPrefab, position, Quaternion.identity);
        EstatuaDrop estatua = estatuaObj.GetComponent<EstatuaDrop>();
        estatua.storedXP = xpAmount;
        estatua.dropRecover = droppedItems;
        estatua.estatuaID = System.Guid.NewGuid().ToString();

        activeEstatuas.Add(estatua);

        // Salva os dados para recriar depois
        EstatuaData data = new EstatuaData
        {
            estatuaID = estatua.estatuaID,
            position = position,
            storedXP = xpAmount,
            droppedItems = droppedItems
        };
        estatuaDataList.Add(data);

        // Limita a 3 estátuas
        if (activeEstatuas.Count > estatuaLimit)
        {
            EstatuaDrop oldest = activeEstatuas[0];
            EstatuaData oldestData = estatuaDataList[0];
            Debug.Log($"A estátua {oldest.estatuaID} foi perdida permanentemente!");

            Destroy(oldest.gameObject);
            activeEstatuas.RemoveAt(0);
            estatuaDataList.RemoveAt(0);
        }
    }

    public void RecoverEstatua(EstatuaDrop estatua)
    {
        Debug.Log($"Jogador recuperou estátua {estatua.estatuaID} com {estatua.storedXP} XP!");

        activeEstatuas.Remove(estatua);
        estatuaDataList.RemoveAll(x => x.estatuaID == estatua.estatuaID);
        // inventory_System.instance.Add(estatua.storedXP);
        foreach (var item in estatua.dropRecover)
        {
            inventory_System.instance.AddItem(item);
        }

        inventory_System.instance.SaveInventory();
        Destroy(estatua.gameObject);
    }

    private void RecreateEstatuas()
    {
        activeEstatuas.Clear();

        foreach (var data in estatuaDataList)
        {
            GameObject estatuaObj = Instantiate(estatuaPrefab, data.position, Quaternion.identity);
            EstatuaDrop estatua = estatuaObj.GetComponent<EstatuaDrop>();
            estatua.estatuaID = data.estatuaID;
            estatua.storedXP = data.storedXP;
            estatua.dropRecover = data.droppedItems;

            activeEstatuas.Add(estatua);
        }

        Debug.Log($"Recriou {activeEstatuas.Count} estátuas na cena {SceneManager.GetActiveScene().name}");
    }
}
