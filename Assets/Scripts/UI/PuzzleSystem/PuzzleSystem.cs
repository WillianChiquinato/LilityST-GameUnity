using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum PuzzlePieceType
{
    HandBase,
    LongBase,
    MidBase,
    FinalPieceEye,
    None
}

public class PuzzleSystem : MonoBehaviour
{
    public static PuzzleSystem instance;

    [Header("Puzzle Container")]
    [SerializeField] private Transform puzzleContainer;

    [Header("Puzzle Data")]
    public List<PuzzleData> puzzlesNaArea = new();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        puzzleContainer = GameObject.Find("PuzzleManager").transform;
        puzzlesNaArea = new List<PuzzleData>(
            puzzleContainer.GetComponentsInChildren<PuzzleData>()
        );
    }

    public void OnPuzzleCompleted(PuzzleData puzzle)
    {
        Debug.LogWarning($"Puzzle finalizado: {puzzle.puzzleID}");

        if (puzzle.puzzleID == "EstatuaLuvas")
        {
            Debug.LogWarning($"Quebrando CHAO");
            StartCoroutine(QuebrarChao(puzzle.gameObject, puzzle));
        }
    }

    public IEnumerator QuebrarChao(GameObject puzzleObj, PuzzleData puzzle)
    {
        Debug.Log("Quebrando o chão da estátua...");
        Transform ultimoFilho = puzzleObj.transform.GetChild(
            puzzleObj.transform.childCount - 1
        );
        ultimoFilho.GetComponent<Animator>().SetTrigger("Quebrar");
        yield return new WaitForSeconds(0.08f);

        GameManager.instance.shakeCamera.ShakeHitDamage();
        yield return new WaitForSeconds(0.25f);
        GameManager.instance.shakeCamera.ShakeHitDamage();
        yield return new WaitForSeconds(0.25f);
        GameManager.instance.shakeCamera.ShakeHitDamage();
        yield return new WaitForSeconds(0.25f);

        ultimoFilho.GetComponent<SpriteRenderer>().enabled = false;

        GameObject pontoQuebraChao = puzzle.Reference;

        if (RemovendoTile.instance == null || pontoQuebraChao == null)
        {
            Debug.LogError("RemovendoTile ou 'pontoQuebraChao' não configurado.");
            yield break;
        }

        Tilemap tilemapChao = RemovendoTile.instance.ObterTilemap(RemovendoTile.TipoTilemap.Chao);

        // Usa a posição do MARCADOR, não do RemovendoTile/GameManager
        Vector3Int celulaQuebra = tilemapChao.WorldToCell(pontoQuebraChao.transform.position);

        Debug.Log($"BreakPoint World: {pontoQuebraChao.transform.position} | BreakPoint Cell: {celulaQuebra}");

        const int tilesParaCadaLado = 7;

        Vector3Int startCell = new Vector3Int(
            celulaQuebra.x - tilesParaCadaLado,
            celulaQuebra.y,
            0
        );

        Vector2Int size = new Vector2Int(
            tilesParaCadaLado * 2 + 1,
            1
        );

        Debug.Log(
            $"Quebrando chão: X {startCell.x} → {startCell.x + size.x - 1} | Y {startCell.y}"
        );

        RemovendoTile.instance.RemoverArea(
            RemovendoTile.TipoTilemap.Chao,
            startCell,
            size
        );
        yield return null;
    }
}
