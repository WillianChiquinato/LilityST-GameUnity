using UnityEngine;

public class PlayerArco : MonoBehaviour
{
    [Header("Partes do Corpo")]
    public Transform tronco;
    public Transform cabeca;

    [Header("Braço")]
    public Transform bracoDireito;
    public Transform bracoEsquerdo;
    public Transform ArcoBracoEsquerdo;

    [Header("Pesos de movimento")]
    public float troncoPeso = 0.3f;
    public float cabecaPeso = 0.1f;

    [Header("Limites")]
    public float troncoMax = 15f;
    public float cabecaMax = 10f;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 direction = mousePos - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Braço segue totalmente
        bracoDireito.rotation = Quaternion.Euler(0, 0, angle);
        bracoEsquerdo.rotation = Quaternion.Euler(0, 0, angle);
        ArcoBracoEsquerdo.rotation = Quaternion.Euler(0, 0, angle);

        float troncoAngle = Mathf.Clamp(angle * troncoPeso, -troncoMax, troncoMax);
        tronco.localRotation = Quaternion.Euler(0, 0, troncoAngle);
        float cabecaAngle = Mathf.Clamp(angle * cabecaPeso, -cabecaMax, cabecaMax);
        cabeca.localRotation = Quaternion.Euler(0, 0, cabecaAngle);
    }
}
