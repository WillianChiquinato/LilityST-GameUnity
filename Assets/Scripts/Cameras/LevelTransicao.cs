using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-1000)]
public class LevelTransicao : MonoBehaviour
{
    public Animator animator;

    public void Transicao(string sceneName)
    {
        StartCoroutine(loadScene(sceneName));
    }

    IEnumerator loadScene(string sceneName)
    {
        animator.SetTrigger("start");

        yield return new WaitForSeconds(1.1f);

        SceneManager.LoadScene(sceneName);
    }
}
