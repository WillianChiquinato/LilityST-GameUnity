using System.Collections;
using UnityEngine;

public class Candelabro : MonoBehaviour
{
    [Header("Configurações")]
    private float offsetY = 0.2f;
    private float speed = 5f;
    private float delay = 0.2f;

    private Vector3 startPos;
    private bool isMoving = false;

    private void Start()
    {
        startPos = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !isMoving)
        {
            StartCoroutine(MovePlatform());
        }
    }

    private IEnumerator MovePlatform()
    {
        isMoving = true;

        Vector3 targetPos = startPos + Vector3.down * offsetY;

        // Descer
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // Espera um pouco antes de subir
        yield return new WaitForSeconds(delay);

        // Subir
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * speed;
            transform.position = Vector3.Lerp(targetPos, startPos, t);
            yield return null;
        }

        isMoving = false;
    }
}
