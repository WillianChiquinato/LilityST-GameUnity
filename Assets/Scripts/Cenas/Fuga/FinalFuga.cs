using System.Collections;
using UnityEngine;

public class FinalFuga : MonoBehaviour
{
    [Header("Player & Instances")]
    public LevelTransicao levelTransicao;
    public PlayerMoviment playerMoviment;
    public Transform positionGeneral;
    public GameObject generalPrefab;
    public GameObject GeneralInstance;

    public GameObject LançaGeneralPrefab;
    public GameObject LançaGeneral = null;
    public Transform PosicaoLançaGeneral;
    public bool LançaTrigger = false;
    private bool isMovingAutomatically = false;

    void Awake()
    {
        playerMoviment = GameObject.FindFirstObjectByType<PlayerMoviment>();
    }

    void Update()
    {
        if (LançaTrigger && !isMovingAutomatically) 
        {
            StartCoroutine(AutoMove(3f));
        }
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
        }
    }

    IEnumerator AutoMove(float duration)
    {
        isMovingAutomatically = true;
        playerMoviment.canMove = false;

        float timer = 0f;
        while (timer < duration)
        {
            playerMoviment.transform.Translate(Vector2.left * playerMoviment.maxSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            playerMoviment.IsMoving = true;
            yield return null;
        }
        playerMoviment.IsMoving = false;

        yield return new WaitForSeconds(1.5f);
        playerMoviment.IsRight = true;

        LançaTrigger = false;
        if (GeneralInstance == null)
        {
            GeneralInstance = Instantiate(generalPrefab, positionGeneral.position, generalPrefab.transform.rotation);
        }

        yield return new WaitForSeconds(2f);

        //Trocar posteriormente para a cena final
        levelTransicao.gameObject.SetActive(true);
        levelTransicao.Transicao("DimensaoTempo");
    }
}
