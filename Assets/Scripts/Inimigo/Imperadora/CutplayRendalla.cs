using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CutplayRendalla : MonoBehaviour
{
    [Header("Instancias")]
    public PlayerMoviment player;
    public Animator animacaoRendalla;
    public GameObject interfaceLilith;
    public GameObject Rendalla;
    public RobertMoviment Robert;
    public GameObject spawnRendalla;
    public GameObject chao;

    [Header("Transicao da camera")]
    public CinemachineVirtualCamera cinemachineVirtualCamera;
    public CinemachineFramingTransposer framingPosition;
    public ShakeCamera shakeCamera;


    public GameObject prefabGeneralMelee;
    public GameObject prefabGeneralLanceiro;
    public GameObject prefabGeneralLanceiro2;
    public GameObject spawnSoldados;
    public GameObject spawnSoldados2;
    public GameObject spawnSoldados3;
    public GameObject spawnRendallaSecurity;
    private bool IsCutplayAutomatically = false;

    [Header("Troca de shaders")]
    public Material shaderCutplay;
    public Material shaderNormal;

    public LevelTransicao levelTransicao;

    void Start()
    {
        player = FindFirstObjectByType<PlayerMoviment>();
        Robert = FindFirstObjectByType<RobertMoviment>();
        levelTransicao = FindFirstObjectByType<LevelTransicao>();

        framingPosition = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        shakeCamera = cinemachineVirtualCamera.GetComponent<ShakeCamera>();
    }

    void Update()
    {
        if (Dialogos_Manager.dialogos_Manager.finishedDialogo && IsCutplayAutomatically)
        {
            StartCoroutine(ImperadoraPosDialog());
            IsCutplayAutomatically = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ImperadoraSpawnCutPlay(1.8f));
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

        interfaceLilith.SetActive(false);
        framingPosition.m_TrackedObjectOffset = new Vector3(framingPosition.m_TrackedObjectOffset.x + 6f, framingPosition.m_TrackedObjectOffset.y + 0.4f, 0);
        yield return new WaitForSeconds(2f);

        //TODO: Spawn Imperadora
        if (spawnRendallaSecurity == null)
        {
            spawnRendallaSecurity = Instantiate(Rendalla, spawnRendalla.transform.position, Quaternion.identity);
        }
        animacaoRendalla = spawnRendallaSecurity.GetComponent<Animator>();
        Animator animacaoRobert = Robert.GetComponent<Animator>();
        yield return new WaitForSeconds(0.14f);
        player.animacao.SetBool("Cutplay", true);
        yield return new WaitForSeconds(0.1f);
        shakeCamera.ShakeCutplayRendalla();

        Robert.GetComponent<SpriteRenderer>().material.SetFloat("_HitIntensity", 1f);
        Robert.GetComponent<SpriteRenderer>().material.SetColor("_HitColor", Color.black);

        //Cor preta no player e no Robert
        player.GetComponent<SpriteRenderer>().material.SetFloat("_HitIntensity", 1f);
        player.GetComponent<SpriteRenderer>().material.SetColor("_HitColor", Color.black);
        chao.GetComponent<TilemapRenderer>().material = shaderCutplay;

        yield return new WaitForSeconds(0.25f);
        player.GetComponent<SpriteRenderer>().material.SetFloat("_HitIntensity", 0f);
        Robert.GetComponent<SpriteRenderer>().material.SetFloat("_HitIntensity", 0f);
        chao.GetComponent<TilemapRenderer>().material = shaderNormal;

        yield return new WaitForSeconds(1.4f);
        animacaoRendalla.SetBool("Algemas", true);
        animacaoRobert.SetBool("Algemas", true);

        yield return new WaitForSeconds(2f);
        player.animacao.SetBool("SustoAlgemas", true);
        player.animacao.SetBool("Cutplay", false);

        yield return new WaitForSeconds(3.4f);
        animacaoRendalla.SetBool("Desconfiar", true);
    }

    IEnumerator ImperadoraPosDialog()
    {
        player.canMove = false;

        yield return new WaitForSeconds(1f);
        animacaoRendalla.SetBool("Ordem", true);
        if (prefabGeneralLanceiro != null && prefabGeneralMelee != null && prefabGeneralLanceiro2 != null)
        {
            Instantiate(prefabGeneralMelee, spawnSoldados.transform.position, Quaternion.identity);
            Instantiate(prefabGeneralLanceiro, spawnSoldados2.transform.position, Quaternion.identity);
            Instantiate(prefabGeneralLanceiro2, spawnSoldados3.transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(2f);
        player.AutoMoveAnimations = true;
        player.IsRight = false;
        player.IsMoving = true;
        player.moveInput = new Vector2(-1f, 0f);

        levelTransicao.Transicao("CutScene-Video");
    }
}
