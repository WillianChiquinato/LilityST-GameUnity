using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
public class PlayerAttack : MonoBehaviour
{
    [Header("Attack references")]
    private PlayerMoviment PlayerCaracter;
    private Rigidbody2D rb;
    public GameObject hitEffectPrefab;
    public GameObject hitEffectPosition;
    public GameObject hitEffect;


    [Header("Attack Variables")]
    public float defaultForce = 15;
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
        if (collision.gameObject.CompareTag("Inimigos"))
        {
            Damage damage = collision.gameObject.GetComponent<Damage>();
            Debug.Log("Pegou " + damage);

            PlayerPoco attack = collision.GetComponent<PlayerPoco>();

            ApplyDamage(damage);

            if (attack != null)
            {
                HandleCollision(attack);
            }

            // Verifica se o inimigo é um slime
            SlimeMoviment slime = collision.gameObject.GetComponent<SlimeMoviment>();
            if (slime != null)
            {
                defaultForce += slime.scaleFactor;
            }
        }
    }

    private void HandleCollision(PlayerPoco AttackPoco)
    {
        if (AttackPoco.giveUpwardForce)
        {
            if (Input.GetKey(KeyCode.W) && !PlayerCaracter.touching.IsGrouded)
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
        }

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
        defaultForce = 15;
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

        if (hitEffect == null)
        {
            hitEffect = Instantiate(hitEffectPrefab, hitEffectPosition.transform.position, Quaternion.identity);
            Destroy(hitEffect, 0.3f);
        }
    }
}
