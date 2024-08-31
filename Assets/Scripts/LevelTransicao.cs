using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTransicao : MonoBehaviour
{
    public Animator animator;

    public void Transicao(string sceneName)
    {
        StartCoroutine(loadScene(sceneName));
        Debug.Log("iniciando");
    }

    IEnumerator loadScene(string sceneName)
    {
        animator.SetTrigger("start");

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene(sceneName);
    } 
}
