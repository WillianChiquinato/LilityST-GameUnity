using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WorldMapData", menuName = "Map/World Map Data")]
public class WorldMapData : ScriptableObject
{
    [Serializable]
    public struct WorldTileEntry
    {
        public int x;
        public int y;
        public int tileId;
    }

    [Serializable]
    public struct TileSpriteEntry
    {
        public int tileId;
        public Sprite sprite;
    }

    [Serializable]
    public struct WorldSpriteEntry
    {
        public int x;
        public int y;
        public Sprite sprite;
    }

    [SerializeField] private List<WorldTileEntry> tiles = new List<WorldTileEntry>();
    [SerializeField] private List<TileSpriteEntry> tileSprites = new List<TileSpriteEntry>();
    [SerializeField] private List<WorldSpriteEntry> spriteTiles = new List<WorldSpriteEntry>();

    private readonly Dictionary<Vector2Int, int> tileLookup = new Dictionary<Vector2Int, int>();
    private readonly Dictionary<int, Sprite> spriteLookup = new Dictionary<int, Sprite>();
    private readonly Dictionary<Vector2Int, Sprite> spriteCellLookup = new Dictionary<Vector2Int, Sprite>();
    private bool cacheBuilt;

    public void ClearAll()
    {
        tiles.Clear();
        spriteTiles.Clear();
        tileLookup.Clear();
        spriteCellLookup.Clear();
        cacheBuilt = false;
    }

    public bool TryGetTileSprite(Vector2Int cell, out Sprite sprite)
    {
        EnsureCache();
        return spriteCellLookup.TryGetValue(cell, out sprite);
    }

    public void SetTileSprite(Vector2Int cell, Sprite sprite)
    {
        EnsureCache();

        if (!sprite)
        {
            RemoveTile(cell);
            return;
        }

        spriteCellLookup[cell] = sprite;

        for (int index = 0; index < spriteTiles.Count; index++)
        {
            if (spriteTiles[index].x != cell.x || spriteTiles[index].y != cell.y)
                continue;

            spriteTiles[index] = new WorldSpriteEntry
            {
                x = cell.x,
                y = cell.y,
                sprite = sprite
            };
            return;
        }

        spriteTiles.Add(new WorldSpriteEntry
        {
            x = cell.x,
            y = cell.y,
            sprite = sprite
        });
    }

    public bool TryGetTileId(Vector2Int cell, out int tileId)
    {
        EnsureCache();
        return tileLookup.TryGetValue(cell, out tileId);
    }

    public void SetTileId(Vector2Int cell, int tileId)
    {
        EnsureCache();

        tileLookup[cell] = tileId;

        for (int index = 0; index < tiles.Count; index++)
        {
            if (tiles[index].x != cell.x || tiles[index].y != cell.y)
                continue;

            tiles[index] = new WorldTileEntry
            {
                x = cell.x,
                y = cell.y,
                tileId = tileId
            };
            return;
        }

        tiles.Add(new WorldTileEntry
        {
            x = cell.x,
            y = cell.y,
            tileId = tileId
        });
    }

    public void RemoveTile(Vector2Int cell)
    {
        EnsureCache();
        tileLookup.Remove(cell);
        spriteCellLookup.Remove(cell);

        for (int index = tiles.Count - 1; index >= 0; index--)
        {
            if (tiles[index].x == cell.x && tiles[index].y == cell.y)
            {
                tiles.RemoveAt(index);
            }
        }

        for (int index = spriteTiles.Count - 1; index >= 0; index--)
        {
            if (spriteTiles[index].x == cell.x && spriteTiles[index].y == cell.y)
            {
                spriteTiles.RemoveAt(index);
            }
        }
    }

    public Sprite GetSpriteByTileId(int tileId)
    {
        EnsureCache();
        return spriteLookup.TryGetValue(tileId, out Sprite sprite) ? sprite : null;
    }

    public IEnumerable<WorldTileEntry> GetTiles()
    {
        return tiles;
    }

    private void EnsureCache()
    {
        if (cacheBuilt)
            return;

        tileLookup.Clear();
        for (int index = 0; index < tiles.Count; index++)
        {
            WorldTileEntry entry = tiles[index];
            tileLookup[new Vector2Int(entry.x, entry.y)] = entry.tileId;
        }

        spriteLookup.Clear();
        for (int index = 0; index < tileSprites.Count; index++)
        {
            TileSpriteEntry entry = tileSprites[index];
            spriteLookup[entry.tileId] = entry.sprite;
        }

        spriteCellLookup.Clear();
        for (int index = 0; index < spriteTiles.Count; index++)
        {
            WorldSpriteEntry entry = spriteTiles[index];
            spriteCellLookup[new Vector2Int(entry.x, entry.y)] = entry.sprite;
        }

        cacheBuilt = true;
    }

    private void OnEnable()
    {
        cacheBuilt = false;
    }

    private void OnValidate()
    {
        cacheBuilt = false;
    }
}