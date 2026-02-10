using System.Collections;
using UnityEngine;

public class BastaoController : MonoBehaviour
{
    public TriggerActiveAnim triggerActiveAnim;
    public float pickupDistance = 1.5f;
    private bool isPickingUp;

    void Start()
    {
        triggerActiveAnim = GetComponent<TriggerActiveAnim>();
    }

    void Update()
    {
        if (triggerActiveAnim.activeTriggerAnim && !isPickingUp)
        {
            isPickingUp = true;
            GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);

            float weaponX = transform.position.x;
            float playerX = GameManager.instance.player.transform.position.x;

            float side = Mathf.Sign(playerX - weaponX);
            if (side == 0) side = 1;

            float targetX = weaponX + side * pickupDistance;
            bool faceRightToWeapon = weaponX > playerX;
            StartCoroutine(PickupRoutine(targetX, faceRightToWeapon, triggerActiveAnim.animationName));
        }
    }

    IEnumerator PickupRoutine(float targetX, bool faceRightToWeapon, string animation)
    {
        GameManager.instance.player.AutoMoveAnimations = true;
        GameManager.instance.player.canMove = false;
        Transform playerTransform = GameManager.instance.player.transform;
        Rigidbody2D playerRb = GameManager.instance.player.rb;
        GameManager.instance.player.IsRight = faceRightToWeapon;
        
        GameManager.instance.player.moveInput = Vector2.zero;
        GameManager.instance.player.rb.linearVelocity = new Vector2(0f, GameManager.instance.player.rb.linearVelocity.y);

        while (Mathf.Abs(playerRb.position.x - targetX) > 0.05f)
        {
            float newX = Mathf.MoveTowards(playerRb.position.x, targetX, GameManager.instance.player.maxSpeed * Time.fixedDeltaTime);
            playerRb.MovePosition(new Vector2(newX, playerRb.position.y));

            GameManager.instance.player.IsMoving = true;
            yield return null;
        }

        GameManager.instance.player.rb.linearVelocity = new Vector2(0f, GameManager.instance.player.rb.linearVelocity.y);
        GameManager.instance.player.moveInput = Vector2.zero;
        GameManager.instance.player.IsMoving = false;
        GameManager.instance.player.IsRight = faceRightToWeapon;
        playerTransform.position = new Vector3(
            targetX,
            playerTransform.position.y,
            playerTransform.position.z
        );
        
        yield return new WaitForSeconds(0.6f);

        if (triggerActiveAnim.boolMode)
        {
            GameManager.instance.player.animacao.SetBool(animation, true);
        }
        else
        {
            GameManager.instance.player.animacao.SetTrigger(animation);
        }

        yield return new WaitForSeconds(0.8f);
        gameObject.GetComponent<SpriteRenderer>().enabled = false;

        yield return new WaitForSeconds(0.94f);
        GameManager.instance.shakeCamera.ShakeHitDamage();

        GameManager.instance.player.animacao.SetBool(animation, false);
        GameManager.instance.player.AutoMoveAnimations = false;
        isPickingUp = false;
        gameObject.SetActive(false);
    }
}
