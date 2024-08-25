using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [SerializeField]
    public static DontDestroy Instance { get; private set; }
    public string CurrentSceneName { get; set; }

    [SerializeField]
    private string initialSceneName;

    public Sistema_Pause sistema_Pause;
    public GameObject sistemaPausePrefab;

    public void Awake()
    {
        //Cena do começo
        CurrentSceneName = initialSceneName;

        sistema_Pause = FindObjectOfType<Sistema_Pause>();
        if (sistema_Pause == null && sistemaPausePrefab != null)
        {
            GameObject sistemaPauseInstance = Instantiate(sistemaPausePrefab);
            sistema_Pause = sistemaPauseInstance.GetComponent<Sistema_Pause>();
        }

        if (sistema_Pause != null && sistema_Pause.IrMenu == true)
        {
            foreach (var longLifeObj in sistema_Pause.objs)
            {
                Destroy(longLifeObj);
            }
        }
    }


    //Deixando para teste, apenas para a build
    public void ResetDestroy()
    {
        if (sistema_Pause.objs.Length > 6)
        {
            // Se já existir, destrói o objeto atual para evitar duplicados
            Destroy(this.gameObject);
        }
        else
        {
            // Se não existir, mantém o objeto ao mudar de cena
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
