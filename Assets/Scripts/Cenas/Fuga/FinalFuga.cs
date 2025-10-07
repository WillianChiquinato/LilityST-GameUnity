using System.Collections;
using UnityEngine;

public class FinalFuga : MonoBehaviour
{
    [Header("Player & Instances")]
    public LevelTransicao levelTransicao;
    public GameObject telaFimFuga;
    public Transform positionGeneral;
    public GameObject generalPrefab;
    public GameObject GeneralInstance;

    public GameObject LançaGeneralPrefab;
    public GameObject LançaGeneral = null;
    public Transform PosicaoLançaGeneral;
    public bool LançaTrigger = false;
    public bool isMovingAutomatically = false;

    void Start()
    {
        telaFimFuga.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LançaTrigger = true;
            if (LançaGeneral == null)
            {
                LançaGeneral = Instantiate(LançaGeneralPrefab, PosicaoLançaGeneral.position, LançaGeneralPrefab.transform.rotation);
            }

            if (LançaTrigger && !isMovingAutomatically)
            {
                StartCoroutine(AutoMove(1.7f));
            }
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.instance.playerMoviment.moveInput = new Vector2(-1f, 0f);
        }
    }

    IEnumerator AutoMove(float duration)
    {
        isMovingAutomatically = true;
        GameManager.instance.playerMoviment.AutoMoveAnimations = true;
        GameManager.instance.playerMoviment.canMove = false;
        GameManager.instance.playerMoviment.moveInput = new Vector2(-1f, 0f);

        float timer = 0f;
        Rigidbody2D rb = GameManager.instance.playerMoviment.GetComponent<Rigidbody2D>();

        while (timer < duration)
        {
            rb.linearVelocity = new Vector2(GameManager.instance.playerMoviment.maxSpeed, rb.linearVelocity.y);
            GameManager.instance.playerMoviment.IsMoving = true;

            timer += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        GameManager.instance.playerMoviment.IsMoving = false;
        GameManager.instance.playerMoviment.AutoMoveAnimations = false;

        yield return new WaitForSeconds(1.5f);
        GameManager.instance.playerMoviment.IsRight = true;

        LançaTrigger = false;

        yield return new WaitForSeconds(2f);
        if (GeneralInstance == null)
        {
            GeneralInstance = Instantiate(generalPrefab, positionGeneral.position, generalPrefab.transform.rotation);
        }

        Vector3 diferrenca = GeneralInstance.transform.position - GameManager.instance.playerMoviment.transform.position;
        GameManager.instance.framingPosition.m_TrackedObjectOffset = new Vector3(diferrenca.x - 2.5f, GameManager.instance.framingPosition.m_TrackedObjectOffset.y, 0);

        yield return new WaitForSeconds(2f);
        GeneralInstance.GetComponent<Animator>().SetBool("FinalFuga", true);

        yield return new WaitForSeconds(1.1f);
        telaFimFuga.SetActive(true);

        yield return new WaitForSeconds(2.8f);
        GeneralInstance.GetComponent<Animator>().SetBool("FinalFuga", false);

        yield return new WaitForSeconds(1.3f);
        levelTransicao.gameObject.SetActive(true);
        levelTransicao.Transicao("DimensaoTempo");
    }
}
