using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TilemapWorldDataBaker : MonoBehaviour
{
    [Serializable]
    public struct TileToIdMapping
    {
        public TileBase tile;
        public int tileId;
    }

    public Tilemap sourceTilemap;
    public WorldMapData worldMapData;
    public bool clearDataBeforeBake = true;
    public bool ignoreUnmappedTiles = true;
    public int fallbackTileId;
    public List<TileToIdMapping> mappings = new List<TileToIdMapping>();

    [ContextMenu("Bake Tilemap To World Data")]
    public void Bake()
    {
        if (!sourceTilemap || !worldMapData)
            return;

        Dictionary<TileBase, int> mappingLookup = new Dictionary<TileBase, int>();
        for (int index = 0; index < mappings.Count; index++)
        {
            TileToIdMapping mapping = mappings[index];
            if (!mapping.tile)
                continue;

            mappingLookup[mapping.tile] = mapping.tileId;
        }

        if (clearDataBeforeBake)
            worldMapData.ClearAll();

        BoundsInt bounds = sourceTilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                TileBase tile = sourceTilemap.GetTile(cell);
                if (!tile)
                    continue;

                if (!mappingLookup.TryGetValue(tile, out int tileId))
                {
                    if (ignoreUnmappedTiles)
                        continue;

                    tileId = fallbackTileId;
                }

                worldMapData.SetTileId(new Vector2Int(x, y), tileId);
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(worldMapData);
#endif
    }
}