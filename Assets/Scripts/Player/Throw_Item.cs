using System.Threading;
using UnityEngine;

public class Throw_Item : MonoBehaviour
{
    public Animator animator;
    public GameObject itemArremessar;
    public GameObject newDrop;
    public bool arremessar;
    public bool TimerArremessar;
    public float ForceArremesso;


    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        arremessar = Input.GetKeyDown(KeyCode.Space);

        if (itemArremessar != null)
        {
            if (arremessar && newDrop == null)
            {
                animator.SetBool("Arremessar", true);
            }

            if (TimerArremessar)
            {
                if (newDrop == null)
                {

                    newDrop = Instantiate(itemArremessar, transform.position, Quaternion.identity);
                    Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();
                    float direction = transform.localScale.x > 0 ? 1 : -1;

                    // Aplica uma força para cima e para frente
                    Vector2 force = new Vector2(ForceArremesso * direction, ForceArremesso * 1.5f);
                    rb.AddForce(force, ForceMode2D.Impulse);
                    animator.SetBool("Arremessar", false);
                    Destroy(newDrop, 7f);
                }
            }

            if (newDrop != null)
            {
                Rigidbody2D rb = newDrop.GetComponent<Rigidbody2D>();

                if (rb.linearVelocity.magnitude < 0.1f)
                {
                    newDrop.tag = "ItemArremessar";
                    newDrop = null;
                }
            }
        }
    }
}
