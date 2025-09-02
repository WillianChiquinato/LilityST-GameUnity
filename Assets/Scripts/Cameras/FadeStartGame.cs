using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeStartGame : MonoBehaviour
{
    public LevelTransicao levelTransicao;
    public bool permissao = false;

    [SerializeField]
    private float delayTime;
    public string jsonFilePath = "Assets/Scripts/SaveData/savepoint.json";
    private string OriginalScene = "Altior-Quarto";

    void Awake()
    {
        levelTransicao = FindFirstObjectByType<LevelTransicao>();
        LoadJson();
    }

    void LoadJson()
    {
        if (File.Exists(jsonFilePath))
        {
            string json = File.ReadAllText(jsonFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            if (IsJsonFileEmpty(jsonFilePath))
            {
                Debug.Log("Cena padrao: " + OriginalScene);
            }
            else
            {
                Debug.Log("Cena Atual: " + saveData.currentScene);
            }
        }
        else
        {
            Debug.LogError("Arquivo JSON não encontrado em: " + jsonFilePath);
        }
    }

    bool IsJsonFileEmpty(string path)
    {
        if (File.Exists(path))
        {
            string fileContent = File.ReadAllText(path);

            return string.IsNullOrWhiteSpace(fileContent);
        }
        else
        {
            Debug.LogError("O arquivo JSON não foi encontrado.");
            return true;
        }
    }

    public void ChamarStartGame()
    {
        StartCoroutine(FadeParaStartGame());
    }

    IEnumerator FadeParaStartGame()
    {
        permissao = true;
        yield return new WaitForSeconds(delayTime);

        string json = File.ReadAllText(jsonFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (IsJsonFileEmpty(jsonFilePath))
        {
            levelTransicao.Transicao(OriginalScene);
        }
        else
        {
            levelTransicao.Transicao(saveData.currentScene);
        }
    }
}
