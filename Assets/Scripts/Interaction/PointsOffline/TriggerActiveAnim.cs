using UnityEngine;

public class TriggerActiveAnim : MonoBehaviour
{
    [Header("Animation Settings")]
    private Animator animatorThis;
    public string animationName;
    public bool isAnimSelf = false;

    public bool activeTriggerAnim = false;

    [Header("Modes")]
    public bool triggerMode = false;
    public bool boolMode = false;
    //Evoluindo.

    void Start()
    {
        animatorThis = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<IconIdle>().startPosition = transform.position + new Vector3(0, 1.2f, 0);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && GameManager.instance.player.entrar && GameManager.instance.player.touching.IsGrouded)
        {
            if (isAnimSelf)
            {
                if (triggerMode)
                {
                    animatorThis.SetTrigger(animationName);
                    return;
                }

                animatorThis.SetBool(animationName, true);
                activeTriggerAnim = true;
            }
            else
            {
                activeTriggerAnim = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}
