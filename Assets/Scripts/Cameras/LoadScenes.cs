using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScenes : MonoBehaviour
{
    private LevelTransicao transicao;
    public string sceneName;

    void Start()
    {
        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
    }

    public void MenuScene()
    {
        transicao.Transicao(sceneName);
    }
}
