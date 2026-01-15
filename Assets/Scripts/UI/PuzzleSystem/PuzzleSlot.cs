using UnityEngine;

public class PuzzleSlot : MonoBehaviour
{
    public PuzzlePieceType correctPiece;
    public bool isFilled;
    public Vector2 targetPosition;

    private PuzzleData puzzleData;

    private void Awake()
    {
        puzzleData = GetComponentInParent<PuzzleData>();
    }

    public bool HasPiece()
    {
        return transform.childCount > 0;
    }

    public bool TryPlace(PuzzlePart part)
    {
        if (part.pieceType != correctPiece) return false;

        // Marca a peça como permanentemente no slot
        part.isInPuzzleSlot = true;
        part.isCollected = false;
        part.isPlaced = true;

        // posiciona visualmente a peça
        part.transform.position = (Vector3)targetPosition;
        part.transform.SetParent(transform);
        part.gameObject.SetActive(true);

        //Zera posicao e rotação da peça
        part.transform.localPosition = Vector3.zero;
        part.transform.localRotation = Quaternion.identity;
        Destroy(part.GetComponent<PuzzlePart>());

        puzzleData.CheckPuzzle(part);
        return true;
    }
}
