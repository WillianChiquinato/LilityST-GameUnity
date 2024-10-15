using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiosController : MonoBehaviour
{
    private static AudiosController instance;

    void Awake()
    {
        // Verifica se já existe uma instância do MusicManager
        if (instance == null)
        {
            // Se não existir, mantém este objeto ao carregar uma nova cena
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se já existir, destrói o objeto duplicado
            Destroy(gameObject);
        }
    }
}
