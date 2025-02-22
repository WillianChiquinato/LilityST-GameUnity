using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FadeStartGame : MonoBehaviour
{
    public bool permissao = false;
    private fadeUI fadeUI;

    [SerializeField]
    private float fadeTime;
    public string nomeCena;
    public SaveData saveData;

    void Awake()
    {
        saveData = SaveData.Instance;
    }

    void Start()
    {
        fadeUI = GetComponent<fadeUI>();
        fadeUI.FadeUIOut(fadeTime);

        nomeCena = saveData.currentScene;
        Debug.Log("Nome da cena: " + nomeCena);
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
        SceneManager.LoadScene(nomeCena);
    }
}
