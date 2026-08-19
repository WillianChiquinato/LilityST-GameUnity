using UnityEngine;
using UnityEngine.Tilemaps;

[DefaultExecutionOrder(-1)]
public class RemovendoTile : MonoBehaviour
{
    public enum TipoTilemap
    {
        Chao,
        Parede,
        Teto
    }

    public static RemovendoTile instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap chao;
    [SerializeField] private Tilemap parede;
    [SerializeField] private Tilemap teto;

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

    public void RemoverArea(
        TipoTilemap tipoTilemap,
        Vector3Int startCell,
        Vector2Int size)
    {
        Tilemap tilemap = ObterTilemap(tipoTilemap);

        if (tilemap == null)
        {
            Debug.LogWarning($"Tilemap '{tipoTilemap}' não foi configurado.");
            return;
        }

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector3Int cell = new Vector3Int(
                    startCell.x + x,
                    startCell.y + y,
                    0
                );

                tilemap.SetTile(cell, null);
            }
        }
    }

    public Tilemap ObterTilemap(TipoTilemap tipoTilemap)
    {
        return tipoTilemap switch
        {
            TipoTilemap.Chao => chao,
            TipoTilemap.Parede => parede,
            TipoTilemap.Teto => teto,
            _ => null
        };
    }
}