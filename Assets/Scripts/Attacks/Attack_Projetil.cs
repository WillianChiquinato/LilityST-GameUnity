using UnityEngine;

public class Attack_Projetil : MonoBehaviour, Defender
{
    public int attackDamage = 1;
    public Vector2 knockback = Vector2.zero;
    private Rigidbody2D rb;

    public bool IsPlayerTarget = false;
    PlayerMoviment player;
    DroggoScript droggoScript;

    [field: SerializeField]
    public float returnSpeed { get; set; } = 26f;

    public float timerDestroyed = 0f;


    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindAnyObjectByType<PlayerMoviment>();
        droggoScript = GameObject.FindAnyObjectByType<DroggoScript>();
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        Damage damage = Collision.GetComponent<Damage>();

        if (damage != null)
        {
            Vector2 projectileKnockback = GetProjectileKnockback(Collision);

            // ataque ao alvo
            bool goHit = damage.Hit(attackDamage, projectileKnockback, transform);
            if (goHit)
            {
                Debug.Log("AtaqueProject");
            }
        }

        if (Collision.CompareTag("Parry"))
        {
            Defender(transform.localScale);
        }

        if (IsPlayerTarget)
        {
            GameManager.instance.shakeCamera.ShakeAttackPlayer();
        }

        if (timerDestroyed > 0f)
        {
            Destroy(gameObject, timerDestroyed);
        }
    }

    private Vector2 GetProjectileKnockback(Collider2D collision)
    {
        float horizontalDirection = 0f;

        if (rb != null && Mathf.Abs(rb.linearVelocity.x) > 0.01f)
        {
            horizontalDirection = Mathf.Sign(rb.linearVelocity.x);
        }
        else
        {
            horizontalDirection = Mathf.Sign(collision.transform.position.x - transform.position.x);
        }

        if (horizontalDirection == 0f)
        {
            horizontalDirection = transform.position.x <= collision.transform.position.x ? 1f : -1f;
        }

        return new Vector2(Mathf.Abs(knockback.x) * horizontalDirection, knockback.y);
    }

    public void TrocarLayer(GameObject obj, int novaLayer)
    {
        obj.layer = novaLayer;
    }

    public void Defender(Vector2 direcao)
    {
        GameObject myObject = this.gameObject;

        int novaLayer = 9;

        rb.linearVelocity = direcao * returnSpeed;
        TrocarLayer(myObject, novaLayer);

        if (transform.localScale.x == -1)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}
