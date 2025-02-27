using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeStartGame : MonoBehaviour
{
    public bool permissao = false;
    private fadeUI fadeUI;

    [SerializeField]
    private float fadeTime;
    public string jsonFilePath = "Assets/Scripts/SaveData/savepoint.json";
    private string OriginalScene = "Altior-Quarto";

    void Awake()
    {
        LoadJson();
    }

    void Start()
    {
        fadeUI = GetComponent<fadeUI>();
        fadeUI.FadeUIOut(fadeTime);
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
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);

        string json = File.ReadAllText(jsonFilePath);
        SaveData saveData = JsonUtility.FromJson<SaveData>(json);

        if (IsJsonFileEmpty(jsonFilePath))
        {
            SceneManager.LoadScene(OriginalScene);
        }
        else
        {
            SceneManager.LoadScene(saveData.currentScene);
        }
    }
}
