using UnityEngine;
using Cinemachine;

public class Sistema_Cenas : MonoBehaviour
{
    public GameObject Camera_Size;
    public CinemachineVirtualCamera Size_Camera;
    private LevelTransicao transicao;
    public string sceneName;

    void Awake()
    {
        transicao = GameObject.FindFirstObjectByType<LevelTransicao>();
        Camera_Size = GameObject.FindWithTag("MainCamera");
        Size_Camera = Camera_Size.GetComponentInChildren<CinemachineVirtualCamera>();
    }

    public void OnTriggerEnter2D(Collider2D collisaoEnter)
    {
        if (collisaoEnter.CompareTag("Player"))
        {
            transicao.Transicao(sceneName);
            GameManager.instance.player.playerInput.enabled = false;
        }
    }
}
