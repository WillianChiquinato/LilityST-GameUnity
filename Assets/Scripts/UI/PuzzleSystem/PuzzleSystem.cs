using System.Collections.Generic;
using UnityEngine;

public enum PuzzlePieceType
{
    Head,
    Body,
    ArmLeft,
    ArmRight,
    ArmDownRight,
    ArmDownLeft,
    LegLeft,
    LegRight,
    LegDownRight,
    LegDownLeft,
    FinalPiece,
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
        Debug.Log($"Puzzle finalizado: {puzzle.puzzleID}");

        if (puzzle.puzzleID == "EstatuaMecanica")
        {
            Debug.Log("Quebrar chao e desbloquear Fragment");
        }
    }
}
