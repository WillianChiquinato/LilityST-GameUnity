using UnityEngine;

public class DragaoInputs : MonoBehaviour
{
    public float distanceX;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        distanceX = transform.position.x - GameManager.instance.player.transform.position.x;

        if (distanceX <= 12f && distanceX >= -12f)
        {
            animator.SetBool("PlayerNear", true);
        }
        else
        {
            animator.SetBool("PlayerNear", false);
        }
    }
}
