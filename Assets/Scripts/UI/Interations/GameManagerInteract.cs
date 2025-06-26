using UnityEngine;

public class GameManagerInteract : MonoBehaviour
{
    public static GameManagerInteract Instance { get; set; }

    public PlayerBebe_Moviment player;
    public GameObject interactIcon;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (Sistema_Pause.instance == null)
        {
            player = GameObject.FindFirstObjectByType<PlayerBebe_Moviment>();
        }

        interactIcon = transform.GetChild(0).gameObject;
    }
}
