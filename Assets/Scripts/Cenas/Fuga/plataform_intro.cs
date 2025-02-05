using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class plataform_intro : MonoBehaviour
{
    Rigidbody2D rb;
    public float fallDelay = 2f;

    public float checkdistance;
    public LayerMask layerMaskPlat;
    public PlatformEffector2D platformEffector2D;


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, checkdistance, layerMaskPlat);
        if (hit.collider != null)
        {
            platformEffector2D.useOneWay = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(fallPlataform());
        }
    }

    IEnumerator fallPlataform()
    {
        yield return new WaitForSeconds(0.2f);

        rb.bodyType = RigidbodyType2D.Dynamic;
        Destroy(gameObject, fallDelay);
    }
}
