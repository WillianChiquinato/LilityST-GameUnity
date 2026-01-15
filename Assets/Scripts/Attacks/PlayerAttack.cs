using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack references")]
    private PlayerMoviment PlayerCaracter;
    private Rigidbody2D rb;
    public GameObject hitEffectPrefab;
    public GameObject hitEffectPosition;
    public GameObject hitEffect;

    [Header("Player Knockback")]
    public bool attackReverseKnockback;
    public float playerKnockForce = 5f;
    public float playerKnockDuration = 0.1f;
    private bool isKnocked;


    [Header("Attack Variables")]
    public float defaultForce = 15;
    public float upwardsForce;

    public float movementTime = .2f;
    private Vector2 direction;
    public Vector2 knockback = Vector2.zero;
    public int attackDamage = 1;
    public bool collided;
    private bool downwardStrike;

    public enum AttackType
    {
        Normal,
        Poderoso,
        Especial
    }

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
        if (collision.gameObject.CompareTag("Inimigos") || collision.gameObject.CompareTag("Quebraveis"))
        {
            Damage damage = collision.gameObject.GetComponent<Damage>();

            PlayerPoco attack = collision.GetComponent<PlayerPoco>();

            ApplyDamage(damage, collision.transform);
            ApplyPlayerKnockback();

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

            GameManager.instance.shakeCamera.ShakeAttackPlayer();
        }
    }

    private void HandleCollision(PlayerPoco AttackPoco)
    {
        if (AttackPoco.giveUpwardForce)
        {
            if (Input.GetKey(KeyCode.W))
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

    private void ApplyDamage(Damage damage, Transform target)
    {
        Vector2 flipknockback = transform.parent.localScale.x > 0
            ? knockback
            : new Vector2(-knockback.x, knockback.y);

        bool goHit = damage.Hit(attackDamage, flipknockback, transform.parent);

        if (goHit)
        {
            Debug.Log("AtaqueInimigo");

            // Spawna o efeito diretamente em cima do inimigo
            GameObject hitEffect = Instantiate(
                hitEffectPrefab,
                target.position,
                Quaternion.identity
            );

            Destroy(hitEffect, 0.3f);
        }
    }

    private void ApplyPlayerKnockback()
    {
        if (isKnocked) return;
        if (!attackReverseKnockback) return;

        isKnocked = true;
        PlayerCaracter.DamageScript.VelocityLock = true;

        // Direção inversa à do personagem
        float direction = Mathf.Sign(transform.parent.localScale.x);
        Vector2 knockDir = new Vector2(-direction, 0.2f).normalized;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(knockDir * playerKnockForce, ForceMode2D.Impulse);

        StartCoroutine(ResetPlayerKnock());
    }

    private IEnumerator ResetPlayerKnock()
    {
        yield return new WaitForSeconds(playerKnockDuration);
        isKnocked = false;
        PlayerCaracter.DamageScript.VelocityLock = false;
    }
}
