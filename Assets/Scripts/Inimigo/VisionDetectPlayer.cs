using UnityEngine;

public class VisionDetectPlayer : MonoBehaviour
{
    public enum TipoDeteccao
    {
        Range,
        Visao
    }

    [Header("Configuração")]
    public TipoDeteccao tipoDeteccao = TipoDeteccao.Range;
    public float raioDeteccao = 10f;
    public float anguloVisao = 60f;
    public float distanciaVisao = 10f;
    public LayerMask obstaculoLayer;

    [Header("Persistência de Aggro")]
    [HideInInspector] public bool viuPlayerPrimeiravez = false;
    public bool manterAggroAoPerderVisao = true;
    public float tempoPerderAggro = 3f;

    [SerializeField] private float timerSemVisao;
    private bool playerDetectado;

    public bool PlayerDetectado => playerDetectado;

    public void ResetDetection()
    {
        playerDetectado = false;
        timerSemVisao = 0f;
    }

    public bool Detectar(Vector2 direcaoFrenteInimigo)
    {
        Transform player = GameManager.instance.player.transform;
        Vector2 origem = transform.position;
        Vector2 alvo = player.position;
        Vector2 direcaoAoPlayer = (alvo - origem).normalized;
        float distancia = Vector2.Distance(origem, alvo);

        bool detectadoAgora = false;

        switch (tipoDeteccao)
        {
            case TipoDeteccao.Range:
                detectadoAgora = distancia <= raioDeteccao;
                break;

            case TipoDeteccao.Visao:
                if (distancia <= distanciaVisao)
                {
                    float anguloEntre = Vector2.Angle(direcaoFrenteInimigo, direcaoAoPlayer);
                    if (anguloEntre <= anguloVisao / 2f)
                    {
                        RaycastHit2D[] hits = Physics2D.RaycastAll(origem, direcaoAoPlayer, distancia, obstaculoLayer);
                        detectadoAgora = true;

                        foreach (RaycastHit2D hit in hits)
                        {
                            if (!hit.collider.CompareTag("Ground"))
                            {
                                detectadoAgora = false;
                                break;
                            }
                        }
                    }
                }
                break;
        }

        if (manterAggroAoPerderVisao)
        {
            if (detectadoAgora)
            {
                viuPlayerPrimeiravez = true;
                playerDetectado = true;
                timerSemVisao = 0f;
            }
            else if (playerDetectado)
            {
                timerSemVisao += Time.deltaTime;
                if (timerSemVisao >= tempoPerderAggro)
                    playerDetectado = false;
            }
            return playerDetectado;
        }

        return detectadoAgora;
    }

    //Se o player dar dano no inimigo com ele de costas, tem que aggrar.
    public void DetectarPlayerDeCostas()
    {
        viuPlayerPrimeiravez = true;
        playerDetectado = true;
        timerSemVisao = 0f;
    }

    void OnDrawGizmosSelected()
    {
        if (tipoDeteccao == TipoDeteccao.Range)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, raioDeteccao);
        }
        else
        {
            Gizmos.color = Color.red;
            float direcaoHorizontal = transform.localScale.x > 0 ? -1f : 1f;
            Vector3 direcaoFrente = new Vector3(direcaoHorizontal, 0f, 0f);
            Quaternion rotEsquerda = Quaternion.Euler(0, 0, anguloVisao / 2f);
            Quaternion rotDireita = Quaternion.Euler(0, 0, -anguloVisao / 2f);

            Gizmos.DrawRay(transform.position, rotEsquerda * direcaoFrente * distanciaVisao);
            Gizmos.DrawRay(transform.position, rotDireita * direcaoFrente * distanciaVisao);
        }
    }
}
