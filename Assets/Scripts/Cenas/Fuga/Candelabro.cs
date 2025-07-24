using System.Collections;
using UnityEngine;

public class Candelabro : MonoBehaviour
{
    [Header("Candelabro constraints")]
    Rigidbody2D rb;
    Animator animator;
    public LayerMask layerMaskPlat;
    public PlatformEffector2D platformEffector2D;

    [Header("Candelabro fall")]
    public float fallDelay;
    public float fallDelayTime;
    public bool isFalling = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isFalling)
        {
            fallDelay += Time.deltaTime;
            if (fallDelay >= fallDelayTime / 2)
            {
                animator.SetBool("PreFall", true);
            }

            if (fallDelay >= fallDelayTime)
            {
                StartCoroutine(FallPlataform());
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                Debug.Log("Contact normal: " + contact.normal);
                // if (contact.normal.y < -0.5f)
                // {
                //     isFalling = true;
                // }
            }
        }
    }

    IEnumerator FallPlataform()
    {
        yield return new WaitForSeconds(0.2f);

        rb.bodyType = RigidbodyType2D.Dynamic;
        animator.SetBool("PreFall", false);
        Destroy(gameObject, fallDelay + 10f);
    }
}
