using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DroggoScript : MonoBehaviour
{
    public Animator animator;
    Sistema_Pause sistema_Pause;
    public Transform player;
    public Rigidbody2D rb;
    public CapsuleCollider2D droggoHits;

    public DetectionDroggo detectionZone;
    public Damage damage;
    public Acorda_Boss acorda_Boss;

    private float facingPlayer;
    private bool Olhar = true;
    public float distancia;

    //Fireball droggo
    [SerializeField]
    private Rigidbody2D Fireball;
    [HideInInspector]
    public Rigidbody2D FireballRB;
    public Transform FirebalLocal;
    public Transform Fireball2;

    [SerializeField]
    private float VelocidadeFireBall = 15f;
    private float shootTimerDroggo = 2f;
    public float shootTempo;
    public bool shootTimer;
    

    //Para o Dash
    public bool CanDash = true;
    public bool IsDashing;
    private float ForcaDash = 17f;
    private float DashTimer = 0.2f;
    public float DashCooldown = 1f;


    public bool _Target = false;
    public bool Target
    {
        get { return _Target; }
        private set
        {
            _Target = value;
            animator.SetBool(animationstrings.Target, value);
        }
    }

    public float attackCooldown
    {
        get
        {
            return animator.GetFloat(animationstrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(animationstrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    public float attackCooldownRanged
    {
        get
        {
            return animator.GetFloat(animationstrings.attackCooldownRanged);
        }
        private set
        {
            animator.SetFloat(animationstrings.attackCooldownRanged, Mathf.Max(value, 0));
        }
    }

    public float attackCooldownDash
    {
        get
        {
            return animator.GetFloat(animationstrings.attackCooldownDash);
        }
        private set
        {
            animator.SetFloat(animationstrings.attackCooldownDash, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        sistema_Pause = GameObject.FindAnyObjectByType<Sistema_Pause>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        droggoHits = GetComponent<CapsuleCollider2D>();
        damage = GetComponent<Damage>();
        acorda_Boss = GameObject.FindFirstObjectByType<Acorda_Boss>();
        player = GameObject.FindAnyObjectByType<PlayerMoviment>().GetComponentInChildren<Transform>();

        Fireball2 = Fireball.GetComponentInChildren<Transform>();
        facingPlayer = transform.localScale.x;
    }

    public enum AttackState { Melee, Ranged, Dash }
    public AttackState state;

    void Update()
    {
        shootTempo += Time.deltaTime;

        if(damage.IsAlive == false) 
        {
            StartCoroutine(FimGame()); 
        }

        if (attackCooldown > 0)
        {
            attackCooldown -= Time.deltaTime;
        }
        if (attackCooldownRanged > 0)
        {
            attackCooldownRanged -= Time.deltaTime;
            animator.SetBool(animationstrings.Ranged, false);
        }
        if (attackCooldownDash > 0)
        {
            attackCooldownDash -= Time.deltaTime;
            animator.SetBool(animationstrings.Dash, false);
        }


        if (Olhar == true)
        {
            var olhar = transform.localScale;
            olhar.x = transform.position.x > player.position.x ? facingPlayer : -facingPlayer;
            transform.localScale = olhar;
        }
        else if (IsDashing == false)
        {
            Olhar = true;
        }

        distancia = Mathf.Abs(transform.position.x - player.position.x);

        if (Target = detectionZone.detectColliders.Count > 0)
        {
            state = AttackState.Melee;
        }
        if (distancia < 50f && distancia > 12f)
        {
            state = AttackState.Ranged;
        }
        if (distancia < 12f)
        {
            state = AttackState.Dash;
        }

        else if (transform.localScale.x == 1)
        {
            Fireball2.transform.localScale = new Vector3(1, 1, 1);
        }
        else
        {
            Fireball2.transform.localScale = new Vector3(-1, 1, 1);
        }

        UpdateAttacks();
    }

    private void UpdateAttacks()
    {
        switch (state)
        {
            case AttackState.Melee:
                animator.SetBool(animationstrings.Combate, true);
                break;

            case AttackState.Ranged:
                animator.SetBool(animationstrings.Ranged, true);

                if (shootTimer == true && shootTempo >= shootTimerDroggo)
                {
                    shootTempo = 0;

                    FireballRB = Instantiate(Fireball, FirebalLocal.transform.position, FirebalLocal.transform.rotation);
                    if (transform.localScale.x == 1)
                    {
                        FireballRB.linearVelocity = FireballRB.transform.right * -VelocidadeFireBall;
                    }
                    else
                    {
                        FireballRB.linearVelocity = FireballRB.transform.right * VelocidadeFireBall;
                    }
                }

                break;

            case AttackState.Dash:
                animator.SetBool(animationstrings.Dash, true);

                if (IsDashing == true && CanDash == true)
                {
                    StartCoroutine(Dash());
                }

                break;

            default:
                break;
        }
    }

    public int HitsCounts = 0;

    public void OnHit(int damage, Vector2 knockback)
    {
        HitsCounts++;
        rb.linearVelocity = new Vector2(knockback.x, rb.linearVelocity.y + knockback.y);
    }

    private IEnumerator Dash()
    {
        Olhar = false;
        CanDash = false;
        IsDashing = true;

        float originalGravidade = rb.gravityScale;
        rb.gravityScale = 1f;
        rb.linearVelocity = new Vector2(-transform.localScale.x * ForcaDash, 3f);

        yield return new WaitForSeconds(DashTimer);
        rb.gravityScale = originalGravidade;
        IsDashing = false;

        yield return new WaitForSeconds(DashCooldown);

        CanDash = true;
    }

    private IEnumerator FimGame()
    {
        yield return new WaitForSeconds(2);

        SceneManager.LoadScene("Menu");
        sistema_Pause.IrMenu = true;  
    }
}
