using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WorldSpriteChunkRenderer : MonoBehaviour
{
    public WorldMapData worldMapData;
    public Tilemap sourceTilemap;
    public Vector2Int chunkCoord;
    public Vector2Int chunkSize = new Vector2Int(64, 64);
    public float cellSize = 1f;
    public Vector2 renderOffset;
    public string sortingLayerName = "Default";
    public int sortingOrder;

    private readonly Dictionary<Vector2Int, SpriteRenderer> spawnedSprites = new Dictionary<Vector2Int, SpriteRenderer>();
    private Transform spriteRoot;

    private void Start()
    {
        RebuildChunk();
    }

    [ContextMenu("Rebuild Chunk")]
    public void RebuildChunk()
    {
        bool hasTilemapSource = sourceTilemap;
        bool hasDataSource = worldMapData;

        if (!hasTilemapSource && !hasDataSource)
            return;

        EnsureRoot();

        HashSet<Vector2Int> required = new HashSet<Vector2Int>();
        int baseX = chunkCoord.x * chunkSize.x;
        int baseY = chunkCoord.y * chunkSize.y;

        for (int localX = 0; localX < chunkSize.x; localX++)
        {
            for (int localY = 0; localY < chunkSize.y; localY++)
            {
                int worldX = baseX + localX;
                int worldY = baseY + localY;
                Vector2Int worldCell = new Vector2Int(worldX, worldY);

                Sprite sprite = null;
                if (hasTilemapSource)
                {
                    sprite = sourceTilemap.GetSprite(new Vector3Int(worldX, worldY, 0));
                }
                else if (hasDataSource && worldMapData.TryGetTileSprite(worldCell, out Sprite directSprite))
                {
                    sprite = directSprite;
                }
                else if (hasDataSource && worldMapData.TryGetTileId(worldCell, out int tileId))
                {
                    sprite = worldMapData.GetSpriteByTileId(tileId);
                }

                if (!sprite)
                    continue;

                required.Add(worldCell);
                if (!spawnedSprites.TryGetValue(worldCell, out SpriteRenderer renderer) || !renderer)
                {
                    GameObject tileObj = new GameObject($"Tile_{worldX}_{worldY}");
                    tileObj.transform.SetParent(spriteRoot, false);
                    renderer = tileObj.AddComponent<SpriteRenderer>();
                    spawnedSprites[worldCell] = renderer;
                }

                renderer.sprite = sprite;
                renderer.sortingLayerName = sortingLayerName;
                renderer.sortingOrder = sortingOrder;
                renderer.transform.localPosition = new Vector3(
                    (localX * cellSize) + renderOffset.x,
                    (localY * cellSize) + renderOffset.y,
                    0f
                );
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, SpriteRenderer> pair in spawnedSprites)
        {
            if (required.Contains(pair.Key))
                continue;

            if (pair.Value)
                Destroy(pair.Value.gameObject);

            toRemove.Add(pair.Key);
        }

        for (int index = 0; index < toRemove.Count; index++)
        {
            spawnedSprites.Remove(toRemove[index]);
        }
    }

    [ContextMenu("Clear Chunk")]
    public void ClearChunk()
    {
        foreach (KeyValuePair<Vector2Int, SpriteRenderer> pair in spawnedSprites)
        {
            if (pair.Value)
                Destroy(pair.Value.gameObject);
        }

        spawnedSprites.Clear();
    }

    private void EnsureRoot()
    {
        if (spriteRoot)
            return;

        Transform existing = transform.Find("SpriteRoot");
        if (existing)
        {
            spriteRoot = existing;
            return;
        }

        GameObject root = new GameObject("SpriteRoot");
        root.transform.SetParent(transform, false);
        spriteRoot = root.transform;
    }
}