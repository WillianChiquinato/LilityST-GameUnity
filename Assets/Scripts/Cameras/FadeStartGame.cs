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

    void Start()
    {
        fadeUI = GetComponent<fadeUI>();
        fadeUI.FadeUIOut(fadeTime);
    }

    public void ChamarStartGame(string SceneEntrar)
    {
        StartCoroutine(FadeParaStartGame(SceneEntrar));
    }

    IEnumerator FadeParaStartGame(string SceneEntrar)
    {
        permissao = true;
        fadeUI.FadeUIIn(fadeTime);
        yield return new WaitForSeconds(fadeTime);
        SceneManager.LoadScene(SceneEntrar);
    }
}
