using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudiosController : MonoBehaviour
{
    private static AudiosController instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //Ver a lpgica depois.
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
