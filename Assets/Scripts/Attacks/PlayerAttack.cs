using System.Collections;
using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack references")]
    private PlayerMoviment PlayerCaracter;
    private Rigidbody2D rb;


    [Header("Attack Variables")]
    public float defaultForce = 300;
    public float upwardsForce;

    public float movementTime = .2f;
    private Vector2 direction;
    public Vector2 knockback = Vector2.zero;
    public int attackDamage = 1;
    public bool collided;
    private bool downwardStrike;

    private void Start()
    {
        PlayerCaracter = GetComponentInParent<PlayerMoviment>();
        rb = GetComponentInParent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Damage damage = collision.GetComponent<Damage>();
        PlayerPoco attack = collision.GetComponent<PlayerPoco>();

        if (damage != null)
        {
            ApplyDamage(damage);
        }

        if (attack != null)
        {
            HandleCollision(attack);
        }
    }

    private void HandleCollision(PlayerPoco AttackPoco)
    {
        if (AttackPoco.giveUpwardForce && Input.GetKey(KeyCode.W) && !PlayerCaracter.touching.IsGrouded)
        {
            direction = Vector2.down;
            downwardStrike = true;
            collided = true;
        }
        if (Input.GetKey(KeyCode.S) && !PlayerCaracter.touching.IsGrouded)
        {
            direction = Vector2.up;
            collided = true;
        }

        // //Checks to see if the melee attack is a standard melee attack
        // if (PlayerCaracter.touching.IsGrouded && Mathf.Abs(Input.GetAxis("Vertical")) < 0.1f)
        // {
        //     if (PlayerCaracter.IsRight)
        //     {
        //         direction = Vector2.left;
        //     }
        //     else
        //     {
        //         direction = Vector2.right;
        //     }
        //     collided = true;
        // }

        StartCoroutine(NoLongerColliding());
    }

    private void HandleMovement()
    {
        if (collided)
        {
            if (downwardStrike)
            {
                rb.linearVelocity = direction.normalized * upwardsForce;
            }
            else
            {
                rb.linearVelocity = direction.normalized * defaultForce;
            }
        }
    }

    private IEnumerator NoLongerColliding()
    {
        yield return new WaitForSeconds(movementTime);
        collided = false;
        downwardStrike = false;
    }

    private void ApplyDamage(Damage damage)
    {
        Vector2 flipknockback = transform.parent.localScale.x > 0 ? knockback : new Vector2(-knockback.x, knockback.y);
        bool goHit = damage.hit(attackDamage, flipknockback);
        if (goHit)
        {
            Debug.Log("AtaqueInimigo");
        }
    }
}
