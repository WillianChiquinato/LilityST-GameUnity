using System.Collections.Generic;
using UnityEngine;

public class PuzzleData : MonoBehaviour
{
    [Header("Puzzle Info")]
    public string puzzleID;
    public bool isCompleted;

    [Header("Slots")]
    public List<PuzzleSlot> slots = new();

    private void Awake()
    {
        slots = new List<PuzzleSlot>(GetComponentsInChildren<PuzzleSlot>());
    }

    public void CheckPuzzle(PuzzlePart part)
    {
        if (part.pieceType != PuzzlePieceType.FinalPiece)
        {
            Debug.Log("Peça colocada, mas não é a peça final.");
            return;
        }

        foreach (var slot in slots)
        {
            if (!slot.HasPiece())
            {
                Debug.Log("Peça final colocada, mas ainda existem slots vazios.");
                return;
            }
        }

        CompletePuzzle();
    }

    private void CompletePuzzle()
    {
        if (isCompleted) return;

        isCompleted = true;
        Debug.LogWarning($"Puzzle {puzzleID} concluído!");

        PuzzleSystem.instance.OnPuzzleCompleted(this);
    }

    public void GetSlot(PuzzlePart part)
    {
        foreach (var slot in slots)
        {
            if (slot.TryPlace(part))
            {
                return;
            }
        }

        Debug.LogWarning("Nenhum slot disponível para esta peça de puzzle.");
    }
}
