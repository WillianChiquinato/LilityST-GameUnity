using System.Collections;
using UnityEngine;

public class Quebraveis : MonoBehaviour
{
    public Damage damageHit;

    public SpriteRenderer spriteRenderer;
    public Material newMaterial;
    public Material originalMaterial;

    void Start()
    {
        damageHit = GetComponentInChildren<Damage>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalMaterial = spriteRenderer.material;

        newMaterial = Resources.Load<Material>("Material/Hit");
    }

    public void OnHit(int damage, Vector2 knockback)
    {
        StartCoroutine(OnHitEnemy());
    }

    IEnumerator OnHitEnemy()
    {
        spriteRenderer.material = newMaterial;

        yield return new WaitForSeconds(0.2f);

        spriteRenderer.material = originalMaterial;
    }
}
