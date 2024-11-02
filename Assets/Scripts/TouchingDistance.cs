using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchingDistance : MonoBehaviour
{
    public ContactFilter2D FiltroGround = new ContactFilter2D();
    public ContactFilter2D FiltroWall = new ContactFilter2D();
    public ContactFilter2D FiltroCeiling = new ContactFilter2D();
    public float groundDistancia;
    public float WallDistancia;
    public float CeilingDistancia;

    CapsuleCollider2D touchingCol;
    Animator animator;

    [SerializeField]
    private bool _IsGrouded;
    public bool IsGrouded
    {
        get
        {
            return _IsGrouded;
        }
        set
        {
            _IsGrouded = value;
            animator.SetBool(animationstrings.IsGrouded, value);
        }
    }

    [SerializeField]
    private bool _IsOnWall;
    public bool IsOnWall
    {
        get
        {
            return _IsOnWall;

        }
        private set
        {
            _IsOnWall = value;
            animator.SetBool(animationstrings.IsOnWall, value);
        }
    }

    [SerializeField]
    private bool _IsOnCeiling;
    public bool IsOnCeiling
    {
        get
        {
            return _IsOnCeiling;

        }
        private set
        {
            _IsOnCeiling = value;
            animator.SetBool(animationstrings.IsOnCeiling, value);
        }
    }

    private void Awake()
    {
        touchingCol = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();

        // Obtenha as dimensões do CapsuleCollider2D
        float colliderHeight = touchingCol.size.y;
        float colliderWidth = touchingCol.size.x;

        // Calcule as distâncias de cast baseadas nas dimensões do collider
        groundDistancia = colliderHeight / 4 + 0.01f;
        WallDistancia = colliderWidth / 4 + 0.01f;
        CeilingDistancia = colliderHeight / 4 + 0.01f;
    }

    void FixedUpdate()
    {
        RaycastHit2D[] groundHits = new RaycastHit2D[1];
        RaycastHit2D[] WallHits = new RaycastHit2D[1];
        RaycastHit2D[] CeilingHits = new RaycastHit2D[1];


        // Cast para o chão
        IsGrouded = touchingCol.Cast(Vector2.down, FiltroGround, groundHits, groundDistancia) > 0;
        Debug.DrawRay(touchingCol.bounds.center, Vector2.down * groundDistancia, Color.red);

        // Cast para a parede
        Vector2 wallDirection = gameObject.transform.localScale.x > 0 ? Vector2.right : Vector2.left;
        IsOnWall = touchingCol.Cast(wallDirection, FiltroWall, WallHits, WallDistancia) > 0;
        Debug.DrawRay(touchingCol.bounds.center, wallDirection * WallDistancia, Color.green);

        // Cast para o teto
        IsOnCeiling = touchingCol.Cast(Vector2.up, FiltroCeiling, CeilingHits, CeilingDistancia) > 0;
        Debug.DrawRay(touchingCol.bounds.center, Vector2.up * CeilingDistancia, Color.blue);
    }
}
