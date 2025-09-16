using UnityEngine;

public class FumacaController : MonoBehaviour
{
    public PlayerMoviment playerMoviment;
    public Animator animator;
    private bool wasGrounded = true;

    public GameObject smokePrefabEffectRun;
    public GameObject smokePrefabEffectJump;
    public GameObject smokeEffect;
    public GameObject smokeEffectJump;

    public Vector3 offset;
    public Vector3 offsetJump;

    void Start()
    {
        playerMoviment = GetComponentInParent<PlayerMoviment>();
    }

    private void Update()
    {
        if (playerMoviment.touching.IsGrouded && playerMoviment.IsMoving)
        {
            if (smokeEffect == null)
            {
                smokeEffect = Instantiate(smokePrefabEffectRun, playerMoviment.transform.position + offset, Quaternion.identity);
                Invoke(nameof(ResetSmoke), 0.8f);
            }
        }

        if (playerMoviment.touching.IsGrouded && !wasGrounded)
        {
            if (smokeEffectJump == null)
            {
                smokeEffectJump = Instantiate(smokePrefabEffectJump, playerMoviment.transform.position + offsetJump, Quaternion.identity);
                Invoke(nameof(ResetSmokeJump), 0.3f);
            }
        }

        wasGrounded = playerMoviment.touching.IsGrouded;
    }

    void ResetSmoke()
    {
        if (smokeEffect != null)
        {
            Destroy(smokeEffect);
            smokeEffect = null;
        }
    }

    void ResetSmokeJump()
    {
        if (smokeEffectJump != null)
        {
            Destroy(smokeEffectJump);
            smokeEffectJump = null;
        }
    }
}
