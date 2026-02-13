using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Tilemaps;

public class WorldSpriteChunkLoader : MonoBehaviour
{
    public WorldMapData worldMapData;
    public Tilemap sourceTilemap;
    public bool replicateLoadedSceneTilemaps = true;
    public bool clearDataOnReplicate = false;
    public string ignoredSceneName = "Mapa";
    public WorldSpriteChunkRenderer chunkRendererPrefab;
    public Vector2Int chunkSize = new Vector2Int(64, 64);
    public float cellSize = 1f;
    public int activeChunkRadius = 1;
    public float refreshInterval = 0.2f;

    private readonly Dictionary<Vector2Int, WorldSpriteChunkRenderer> loadedChunks = new Dictionary<Vector2Int, WorldSpriteChunkRenderer>();
    private float refreshTimer;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (!replicateLoadedSceneTilemaps || !worldMapData)
            return;

        Scene activeScene = SceneManager.GetActiveScene();
        ReplicateSceneTilemaps(activeScene);
        RefreshChunks();
    }

    private void Update()
    {
        bool hasDataSource = worldMapData;
        bool hasTilemapSource = sourceTilemap;
        if (!GameManager.instance.player || !chunkRendererPrefab || (!hasDataSource && !hasTilemapSource))
            return;

        refreshTimer -= Time.deltaTime;
        if (refreshTimer > 0f)
            return;

        refreshTimer = refreshInterval;
        RefreshChunks();
    }

    [ContextMenu("Refresh Chunks")]
    public void RefreshChunks()
    {
        bool hasDataSource = worldMapData;
        bool hasTilemapSource = sourceTilemap;
        if (!GameManager.instance.player || !chunkRendererPrefab || (!hasDataSource && !hasTilemapSource))
            return;

        Vector2Int centerChunk = GetChunkCoord(GameManager.instance.player.transform.position);
        HashSet<Vector2Int> required = new HashSet<Vector2Int>();

        for (int dx = -activeChunkRadius; dx <= activeChunkRadius; dx++)
        {
            for (int dy = -activeChunkRadius; dy <= activeChunkRadius; dy++)
            {
                Vector2Int chunkCoord = new Vector2Int(centerChunk.x + dx, centerChunk.y + dy);
                required.Add(chunkCoord);

                if (!loadedChunks.TryGetValue(chunkCoord, out WorldSpriteChunkRenderer chunkRenderer) || !chunkRenderer)
                {
                    chunkRenderer = CreateChunk(chunkCoord);
                    loadedChunks[chunkCoord] = chunkRenderer;
                }

                chunkRenderer.RebuildChunk();
            }
        }

        List<Vector2Int> toRemove = new List<Vector2Int>();
        foreach (KeyValuePair<Vector2Int, WorldSpriteChunkRenderer> pair in loadedChunks)
        {
            if (required.Contains(pair.Key))
                continue;

            if (pair.Value)
                Destroy(pair.Value.gameObject);

            toRemove.Add(pair.Key);
        }

        for (int index = 0; index < toRemove.Count; index++)
        {
            loadedChunks.Remove(toRemove[index]);
        }
    }

    private Vector2Int GetChunkCoord(Vector3 worldPosition)
    {
        float worldChunkWidth = chunkSize.x * cellSize;
        float worldChunkHeight = chunkSize.y * cellSize;

        int chunkX = Mathf.FloorToInt(worldPosition.x / worldChunkWidth);
        int chunkY = Mathf.FloorToInt(worldPosition.y / worldChunkHeight);
        return new Vector2Int(chunkX, chunkY);
    }

    private WorldSpriteChunkRenderer CreateChunk(Vector2Int chunkCoord)
    {
        WorldSpriteChunkRenderer chunkRenderer = Instantiate(chunkRendererPrefab, transform);
        chunkRenderer.name = $"Chunk_{chunkCoord.x}_{chunkCoord.y}";
        chunkRenderer.worldMapData = worldMapData;
        chunkRenderer.sourceTilemap = sourceTilemap;
        chunkRenderer.chunkCoord = chunkCoord;
        chunkRenderer.chunkSize = chunkSize;
        chunkRenderer.cellSize = cellSize;

        chunkRenderer.transform.localPosition = new Vector3(
            chunkCoord.x * chunkSize.x * cellSize,
            chunkCoord.y * chunkSize.y * cellSize,
            0f
        );

        return chunkRenderer;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!replicateLoadedSceneTilemaps || !worldMapData)
            return;

        ReplicateSceneTilemaps(scene);
        RefreshChunks();
    }

    private void ReplicateSceneTilemaps(Scene scene)
    {
        if (!scene.isLoaded)
            return;

        if (!string.IsNullOrWhiteSpace(ignoredSceneName) && scene.name == ignoredSceneName)
            return;

        if (clearDataOnReplicate)
            worldMapData.ClearAll();

        GameObject[] roots = scene.GetRootGameObjects();
        for (int rootIndex = 0; rootIndex < roots.Length; rootIndex++)
        {
            Tilemap[] tilemaps = roots[rootIndex].GetComponentsInChildren<Tilemap>(true);
            for (int tilemapIndex = 0; tilemapIndex < tilemaps.Length; tilemapIndex++)
            {
                ReplicateTilemap(tilemaps[tilemapIndex]);
            }
        }

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(worldMapData);
#endif
    }

    private void ReplicateTilemap(Tilemap tilemap)
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int x = bounds.xMin; x < bounds.xMax; x++)
        {
            for (int y = bounds.yMin; y < bounds.yMax; y++)
            {
                Vector3Int tileCell = new Vector3Int(x, y, 0);
                Sprite sprite = tilemap.GetSprite(tileCell);
                if (!sprite)
                    continue;

                Vector3 worldPosition = tilemap.GetCellCenterWorld(tileCell);
                Vector2Int worldCell = new Vector2Int(
                    Mathf.FloorToInt(worldPosition.x / cellSize),
                    Mathf.FloorToInt(worldPosition.y / cellSize)
                );

                worldMapData.SetTileSprite(worldCell, sprite);
            }
        }
    }
}