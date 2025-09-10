using UnityEngine;

public class PlayerArco : MonoBehaviour
{
    [Header("Partes do Corpo")]
    public Transform tronco;
    public Transform cabeca;
    public Transform tanga;
    public Transform pernas;

    [Header("Braço")]
    public Transform bracoDireito;
    public Transform bracoEsquerdo;
    public Transform ArcoBracoEsquerdo;

    [Header("Pesos de movimento")]
    public float troncoPeso = 0.5f;
    public float cabecaPeso = 0.7f;

    [Header("Limites")]
    public float troncoMax = 50f;
    public float cabecaMax = 35f;
    public float tangaMax = 35f;

    [Header("Velocidade máxima (graus/s)")]
    public float velocidade = 600f;

    void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log(mousePos);
        Vector3 dir = mousePos - transform.position;

        int direction = transform.parent.localScale.x > 0 ? 1 : -1;

        // ângulo absoluto para os braços
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        if (direction < 0)
        {
            angle = 180 + angle;
        }

        angle = NormalizeAngle(angle);

        // Aplica nos braços diretamente
        bracoDireito.rotation = Quaternion.Euler(0, 0, angle);
        bracoEsquerdo.rotation = Quaternion.Euler(0, 0, angle);
        ArcoBracoEsquerdo.rotation = Quaternion.Euler(0, 0, angle);

        // calcula ângulo relativo entre frente do player e o mouse
        Vector2 playerForward = direction > 0 ? Vector2.right : Vector2.left;
        float relativeAngle = Vector2.SignedAngle(playerForward, dir);

        if (direction < 0)
        {
            relativeAngle *= -1;
        }

        // calcula ângulos desejados com limites e pesos
        float troncoTarget = Mathf.Clamp(relativeAngle * troncoPeso, -troncoMax, troncoMax);
        float cabecaTarget = Mathf.Clamp(relativeAngle * cabecaPeso, -cabecaMax, cabecaMax);
        float tangaTarget = Mathf.Clamp(relativeAngle * cabecaPeso, -tangaMax, tangaMax);

        // move suavemente para os alvos
        RotateMember(tronco, troncoTarget);
        RotateMember(cabeca, cabecaTarget);
        RotateMember(tanga, tangaTarget);
    }

    float NormalizeAngle(float angle)
    {
        angle = Mathf.Repeat(angle + 180f, 360f) - 180f;
        return angle;
    }

    void RotateMember(Transform member, float target)
    {
        float currentZ = NormalizeAngle(member.localEulerAngles.z);
        float targetZ = NormalizeAngle(target);

        float newZ = Mathf.MoveTowardsAngle(currentZ, targetZ, velocidade * Time.deltaTime);

        member.localRotation = Quaternion.Euler(0, 0, newZ);
    }
}
