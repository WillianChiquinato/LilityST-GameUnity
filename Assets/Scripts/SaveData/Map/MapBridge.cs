using UnityEngine;

public class MapBridge : MonoBehaviour
{
    public static MapBridge Instance;
    public Transform playerWorld;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
