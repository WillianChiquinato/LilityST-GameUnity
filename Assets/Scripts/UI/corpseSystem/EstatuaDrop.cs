using System.Collections.Generic;
using UnityEngine;

public class EstatuaDrop : MonoBehaviour
{
    public int storedXP;
    public List<ItemData> dropRecover = new List<ItemData>();
    public string estatuaID;

    private void OnTriggerEnter2D(Collider2D collision)
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
        if (collision.CompareTag("Player"))
        {
            if (GameManager.instance.playerMoviment.entrar)
            {
                GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
                GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
                
                EstatuaSystem.Instance.RecoverEstatua(this);
                Debug.LogWarning("Estátua recuperada!");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManagerInteract.Instance.interactIcon.transform.SetParent(GameManagerInteract.Instance.transform);
            GameManagerInteract.Instance.interactIcon.GetComponent<Animator>().SetBool("Visivel", false);
        }
    }
}
