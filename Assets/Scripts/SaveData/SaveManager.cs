using UnityEngine;
using System.IO;

public class SaveManager : MonoBehaviour
{
    private static string saveFilePath = "Assets/Scripts/SaveData/Saves";

    public static void Save(SaveData data, int slot)
    {
        if (!Directory.Exists(saveFilePath))
        {
            Directory.CreateDirectory(saveFilePath);
        }

        string path = saveFilePath + $"save_{slot}.json";
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        Debug.Log($"Save slot {slot} salvo em: " + path);
    }

    public static SaveData Load(int slot)
    {
        string path = saveFilePath + $"save_{slot}.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log($"Save slot {slot} carregado!");
            return data;
        }
        else
        {
            Debug.LogWarning($"Nenhum save encontrado no slot {slot}.");
            return null;
        }
    }

    public static bool SaveExists(int slot)
    {
        string path = saveFilePath + $"save_{slot}.json";
        return File.Exists(path);
    }
}
