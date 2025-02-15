using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static string saveFilePath = "Assets/Scripts/SaveData/savepoint.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(saveFilePath, json);
        Debug.Log("Checkpoint salvo em: " + saveFilePath);
    }

    public static SaveData Load()
    {
        if (File.Exists(saveFilePath))
        {
            string json = File.ReadAllText(saveFilePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("Checkpoint carregado!");
            return data;
        }
        else
        {
            Debug.LogWarning("Nenhum save encontrado.");
            return null;
        }
    }
}
