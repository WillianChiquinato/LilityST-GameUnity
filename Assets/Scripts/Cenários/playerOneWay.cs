using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerOneWay : MonoBehaviour
{
    public GameObject currentOneWayPlat;
    public GameObject[] oficialOneWayPlat;
    public GameObject oneWayStart;
    public PlayerMoviment playerMoviment;

    [SerializeField]
    private CapsuleCollider2D playerCollider;
    public bool plataformOneWay;

    void Start()
    {
        playerMoviment = GameObject.FindObjectOfType<PlayerMoviment>();
        oficialOneWayPlat = GameObject.FindGameObjectsWithTag("OneWayPlataforma");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S) && plataformOneWay == true)
        {
            if (currentOneWayPlat != null)
            {
                StartCoroutine(ColisorDesabilitado());
            }
        }

        if (plataformOneWay)
        {
            foreach (var layers in oficialOneWayPlat)
            {
                layers.layer = 17;
            }
        }
        else
        {
            foreach (var layers in oficialOneWayPlat)
            {
                layers.layer = 0;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D colisor)
    {
        if (colisor.gameObject.CompareTag("OneWayPlataforma"))
        {
            if (oneWayStart.transform.position.y > colisor.transform.position.y)
            {
                currentOneWayPlat = colisor.gameObject;
                plataformOneWay = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D colisor)
    {
        if (colisor.gameObject.CompareTag("OneWayPlataforma"))
        {
            currentOneWayPlat = null;
            plataformOneWay = false;
        }
    }

    IEnumerator ColisorDesabilitado()
    {
        BoxCollider2D plataformCollider = currentOneWayPlat.GetComponent<BoxCollider2D>();
        Physics2D.IgnoreCollision(playerCollider, plataformCollider);

        yield return new WaitForSeconds(0.3f);

        Physics2D.IgnoreCollision(playerCollider, plataformCollider, false);
    }
}
