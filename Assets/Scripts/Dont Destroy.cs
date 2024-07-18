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
    public GameObject[] objs;

    public void Awake()
    {
        sistema_Pause = FindObjectOfType<Sistema_Pause>();
        if (sistema_Pause == null && sistemaPausePrefab != null)
        {
            GameObject sistemaPauseInstance = Instantiate(sistemaPausePrefab);
            sistema_Pause = sistemaPauseInstance.GetComponent<Sistema_Pause>();
        }

        // Verifica se já existe uma instância desse objeto na cena
        objs = GameObject.FindGameObjectsWithTag("DontDestroy");

        if (objs.Length > 8)
        {
            // Se já existir, destrói o objeto atual para evitar duplicados
            Destroy(this.gameObject);
        }
        else
        {
            // Se não existir, mantém o objeto ao mudar de cena
            DontDestroyOnLoad(this.gameObject);
        }

        if (sistema_Pause != null && sistema_Pause.IrMenu == true)
        {
            foreach (var longLifeObj in objs)
            {
                Destroy(longLifeObj);
            }
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Atualiza o nome da cena atual quando uma nova cena é carregada
        CurrentSceneName = scene.name;
    }
}