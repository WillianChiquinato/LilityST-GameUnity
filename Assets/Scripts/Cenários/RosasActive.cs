using UnityEngine;

public class RosasActive : MonoBehaviour
{
    public Animator animator;
    public string tagToCheck = "Player";

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(tagToCheck))
        {
            animator.SetTrigger("Active");
        }
    }
}
