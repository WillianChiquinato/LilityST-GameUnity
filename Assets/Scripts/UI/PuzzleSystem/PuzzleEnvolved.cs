using UnityEngine;

public class PuzzleEnvolved : MonoBehaviour
{
    public bool verifyPartWithPlayer;

    void Update()
    {
        verifyPartWithPlayer = GameManager.instance.player.GetComponentInChildren<PuzzlePart>() != null;
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

    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (verifyPartWithPlayer)
            {
                if (GameManager.instance.player.entrar)
                {
                    gameObject.GetComponentInParent<PuzzleData>().GetSlot(GameManager.instance.player.GetComponentInChildren<PuzzlePart>());
                }
            }
            else
            {
                if (GameManager.instance.player.entrar)
                {
                    GameManager.instance.shakeCamera.ShakeHitDamage();
                    Debug.LogWarning("Sem peça de puzzle para colocar aqui.");
                }
            }
        }
    }
}
