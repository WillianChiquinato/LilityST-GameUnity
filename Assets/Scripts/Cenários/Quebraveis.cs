using System.Collections;
using UnityEngine;

public class Quebraveis : MonoBehaviour
{
    public GameObject OpenQuebraveis;

    public Damage damageHit;
    public Animator animator;
    public int QuantityHealth = 0;

    void Start()
    {
        OpenQuebraveis = transform.GetChild(1).gameObject;
        damageHit = GetComponentInChildren<Damage>();
        animator = GetComponent<Animator>();

        QuantityHealth = damageHit.Health;
    }

    void Update()
    {
        animator.SetInteger("QuantityHit", QuantityHealth);

        if (QuantityHealth == 0)
        {
            Destroy(OpenQuebraveis);
        }
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        QuantityHealth --;
    }
}
