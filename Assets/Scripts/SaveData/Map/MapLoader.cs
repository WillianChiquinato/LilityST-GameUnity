using UnityEngine;
using UnityEngine.SceneManagement;

public class MapLoader : MonoBehaviour
{
    void Start()
    {
        SceneManager.LoadSceneAsync(
            "Mapa",
            LoadSceneMode.Additive
        );
    }
}
