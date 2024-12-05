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

    void Start()
    {
        fadeUI = GetComponent<fadeUI>();
        fadeUI.FadeUIOut(fadeTime);

        nomeCena = SavePoint.nomeCenaMenu;
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
