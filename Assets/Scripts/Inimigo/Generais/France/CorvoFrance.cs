using UnityEngine;

public class CorvoFrance : MonoBehaviour
{
    public Animator animator;
    [HideInInspector] public PlayerMoviment player;
    public float direction;
    public bool esquerdaAnim;

    void Start()
    {
        animator = GetComponent<Animator>();
        player = FindFirstObjectByType<PlayerMoviment>();
    }

    void Update()
    {
        animator.SetBool("EsquerdaCorvo", esquerdaAnim);
        direction = player.transform.position.x - transform.position.x;

        if (direction < 0)
        {
            esquerdaAnim = true;
        }
        else if (direction > 0)
        {
            esquerdaAnim = false;
        }
    }
}
