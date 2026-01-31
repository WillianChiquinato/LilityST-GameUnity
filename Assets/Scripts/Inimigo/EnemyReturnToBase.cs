using System;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyReturnToBase : MonoBehaviour
{
    public float maxDistanceFromBase = 8f;
    public float returnSpeed = 3f;
    public float stopDistance = 0.5f;

    private Vector2 homePosition;
    private Rigidbody2D rb;

    [HideInInspector] public bool returning;

    public float Dist;

    void Awake()
    {
        homePosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Dist = Vector2.Distance(homePosition, transform.position);
    }

    void FixedUpdate()
    {
        if (!returning)
            return;

        if (Dist > maxDistanceFromBase)
            returning = true;

        float direcaoHome = Mathf.Sign(homePosition.x - transform.position.x);

        if (direcaoHome <= stopDistance)
        {
            returning = false;
            rb.linearVelocity = Vector2.zero;
            return;
        }

        rb.linearVelocity = new Vector2(direcaoHome * returnSpeed, rb.linearVelocity.y);;
        transform.localScale = new Vector3(direcaoHome > 0 ? -1 : 1, 1, 1);
    }
}
