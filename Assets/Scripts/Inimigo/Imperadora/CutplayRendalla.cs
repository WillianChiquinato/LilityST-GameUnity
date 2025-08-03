using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CutplayRendalla : MonoBehaviour
{
    [Header("Instancias")]
    public PlayerMoviment player;
    public GameObject Rendalla;
    public RobertMoviment Robert;
    public GameObject spawnRendalla;
    public GameObject chao;

    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;

    public GameObject spawnRendallaSecurity;
    private bool IsCutplayAutomatically = false;

    [Header("Troca de shaders")]
    public Material shaderCutplay;
    public Material shaderNormal;



    void Start()
    {
        player = FindFirstObjectByType<PlayerMoviment>();
        Robert = FindFirstObjectByType<RobertMoviment>();

        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ImperadoraSpawnCutPlay(2.2f));
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player.moveInput = new Vector2(-1f, 0f);
        }
    }

    IEnumerator ImperadoraSpawnCutPlay(float durationLilith)
    {
        IsCutplayAutomatically = true;
        player.AutoMoveAnimations = true;
        player.canMove = false;
        player.moveInput = new Vector2(-1f, 0f);

        float timer = 0f;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        // Força o movimento automático para a esquerda
        while (timer < durationLilith)
        {
            rb.linearVelocity = new Vector2(player.maxSpeed, rb.linearVelocity.y);
            player.IsMoving = true;

            timer += Time.deltaTime;
            yield return null;
        }

        // Para o movimento após a duração
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        player.IsMoving = false;
        player.AutoMoveAnimations = false;

        // Pequena pausa
        yield return new WaitForSeconds(1.5f);

        // Vira o personagem para a direita
        player.IsRight = true;

        yield return new WaitUntil(() => Robert.pointToRendalla);

        yield return new WaitForSeconds(1.5f);

        Debug.Log("Robert ja chegou, agr cutscene");
        framingPosition.m_TrackedObjectOffset = new Vector3(framingPosition.m_TrackedObjectOffset.x + 8f, framingPosition.m_TrackedObjectOffset.y + 0.2f, 0);
        yield return new WaitForSeconds(2f);

        //TODO: Spawn Imperadora
        if (spawnRendallaSecurity == null)
        {
            spawnRendallaSecurity = Instantiate(Rendalla, spawnRendalla.transform.position, Quaternion.identity);
        }
        Animator animacaoRendalla = spawnRendallaSecurity.GetComponent<Animator>();
        Animator animacaoRobert = Robert.GetComponent<Animator>();
        yield return new WaitForSeconds(0.1f);
        player.animacao.SetBool("Cutplay", true);
        player.GetComponent<SpriteRenderer>().material = shaderCutplay;
        Robert.GetComponent<SpriteRenderer>().material = shaderCutplay;
        chao.GetComponent<TilemapRenderer>().material = shaderCutplay;

        yield return new WaitForSeconds(0.1f);
        player.GetComponent<SpriteRenderer>().material = shaderNormal;
        Robert.GetComponent<SpriteRenderer>().material = shaderNormal;
        chao.GetComponent<TilemapRenderer>().material = shaderNormal;

        yield return new WaitForSeconds(1.5f);
        animacaoRendalla.SetBool("Algemas", true);
        animacaoRobert.SetBool("Algemas", true);

        yield return new WaitForSeconds(1f);
        player.animacao.SetBool("SustoAlgemas", true);

        //TODO: Dialogo a seguir.


        //TODO: Transicao para cutplay

    }
}
