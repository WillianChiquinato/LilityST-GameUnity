using UnityEngine;

public class EstalagtiteScript : MonoBehaviour
{
    public Rigidbody2D rbParent;
    public CapsuleCollider2D capsuleCollider2D;

    void Start()
    {
        rbParent = GetComponentInParent<Rigidbody2D>();

        rbParent.bodyType = RigidbodyType2D.Static;
    }
    
    void OnTriggerEnter2D(Collider2D Collision)
    {
        if (Collision.gameObject.CompareTag("Player"))
        {
            rbParent.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
