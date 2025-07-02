using UnityEngine;
using UnityEngine.Tilemaps;

public class RemovendoTile : MonoBehaviour
{
    public Tilemap tilemap;
    public Vector3Int startCell;
    public Vector2Int size;

    public void RemoverArea()
    {
        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = new Vector3Int(startCell.x + x, startCell.y + y, 0);
                tilemap.SetTile(cell, null);
            }
        }
    }
}
