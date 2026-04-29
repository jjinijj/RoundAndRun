using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Tile Prefabs")]
    [SerializeField] private Tile emptyTilePrefab;
    [SerializeField] private Tile obstacleTilePrefab;
    [SerializeField] private Tile itemTilePrefab;

    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private BGThemeData themeData;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private int initialTileCount = 5;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    public Action onTilePassed;

    private Dictionary<TileType, Queue<Tile>> tilePools = new();
    private Dictionary<string, Queue<GameObject>> objectPools = new();
    private Dictionary<string, Queue<GameObject>> itemPools = new();
    private List<Tile> activeTiles = new();
    private int sequenceIndex = 0;
    private bool isRunning = false;
    private LevelData levelData;

    void Start()
    {
        levelData = GetComponent<LevelLoader>().Load();

        InitTilePool(TileType.Empty, emptyTilePrefab);
        InitTilePool(TileType.Obstacle, obstacleTilePrefab);
        InitTilePool(TileType.Item, itemTilePrefab);

        InitObjectPools();
        InitItemPools();
    }

    void InitTilePool(TileType type, Tile prefab)
    {
        tilePools[type] = new Queue<Tile>();
        for (int i = 0; i < 3; i++)
        {
            Tile t = Instantiate(prefab);
            t.gameObject.SetActive(false);
            tilePools[type].Enqueue(t);
        }
    }

    void InitObjectPools()
    {
        foreach (var data in themeData.objects)
        {
            objectPools[data.id] = new Queue<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                var go = Instantiate(data.prefab);
                go.SetActive(false);
                objectPools[data.id].Enqueue(go);
            }
        }
    }

    void InitItemPools()
    {
        if (itemDatabase == null) return;
        foreach (var data in itemDatabase.items)
        {
            itemPools[data.id] = new Queue<GameObject>();
            for (int i = 0; i < 3; i++)
            {
                var go = Instantiate(data.prefab);
                go.SetActive(false);
                itemPools[data.id].Enqueue(go);
            }
        }
    }

    public void StartGame()
    {
        ResetTiles();
        for (int i = 0; i < initialTileCount; i++)
            SpawnTile();
        isRunning = true;
    }

    void ResetTiles()
    {
        foreach (var tile in activeTiles)
        {
            ReturnChildrenToPool(tile);
            tile.gameObject.SetActive(false);
            tilePools[tile.tileType].Enqueue(tile);
        }

        activeTiles.Clear();
        sequenceIndex = 0;
    }

    void ReturnChildrenToPool(Tile tile)
    {
        foreach (var child in tile.DetachAll())
        {
            if (!child.TryGetComponent(out TileObject tileObj)) { Destroy(child); continue; }

            string id = tileObj.Id;
            if (tileObj.Type == TileObjectType.Item && itemPools.ContainsKey(id))
                itemPools[id].Enqueue(child);
            else if (objectPools.ContainsKey(id))
                objectPools[id].Enqueue(child);
            else
                Destroy(child);
        }
    }

    TileData GetNextTileData()
    {
        if (sequenceIndex >= levelData.tiles.Length)
            sequenceIndex = 0;
        return levelData.tiles[sequenceIndex++];
    }

    void SpawnTile()
    {
        TileData tileData = GetNextTileData();
        TileType type = ParseTileType(tileData.tile);
        Tile tile = GetFromTilePool(type);

        float spawnZ = activeTiles.Count > 0
            ? activeTiles[activeTiles.Count - 1].transform.position.z + activeTiles[activeTiles.Count - 1].tileLength
            : 0f;

        tile.transform.position = new Vector3(0, -1f, spawnZ);
        tile.gameObject.SetActive(true);

        if (tileData.objects != null)
        {
            foreach (var objData in tileData.objects)
            {
                var obj = GetObjectFromPools(objData.id);
                if (obj == null) continue;
                if (obj.TryGetComponent(out TileObject tileObj))
                    tileObj.Id = objData.id;
                tile.AttachChild(obj, new Vector3(0, objData.height, 0));
            }
        }

        tile.ResetTile();
        activeTiles.Add(tile);
    }

    GameObject GetObjectFromPools(string id)
    {
        if (objectPools.ContainsKey(id))
            return GetFromPool(objectPools, id, themeData.GetObject(id)?.prefab);
        if (itemPools.ContainsKey(id))
            return GetFromPool(itemPools, id, itemDatabase.GetItem(id)?.prefab);
        return null;
    }

    GameObject GetFromPool(Dictionary<string, Queue<GameObject>> pools, string id, GameObject prefab)
    {
        if (pools.ContainsKey(id) && pools[id].Count > 0)
            return pools[id].Dequeue();
        if (prefab == null) return null;
        return Instantiate(prefab);
    }

    Tile GetFromTilePool(TileType type)
    {
        if (tilePools[type].Count > 0)
            return tilePools[type].Dequeue();
        return Instantiate(GetTilePrefab(type));
    }

    Tile GetTilePrefab(TileType type) => type switch
    {
        TileType.Obstacle => obstacleTilePrefab,
        TileType.Item => itemTilePrefab,
        _ => emptyTilePrefab
    };

    TileType ParseTileType(string tile) => tile switch
    {
        "Obstacle" => TileType.Obstacle,
        "Item" => TileType.Item,
        _ => TileType.Empty
    };

    void Update()
    {
        if (!isRunning) return;
        MoveTiles();
        CheckRecycle();
    }

    void MoveTiles()
    {
        foreach (var tile in activeTiles)
            tile.transform.position += Vector3.back * gameSettings.tileSpeed * Time.deltaTime;
    }

    void CheckRecycle()
    {
        if (activeTiles.Count == 0) return;

        Tile oldest = activeTiles[0];
        if (oldest.transform.position.z >= player.position.z - oldest.tileLength) return;

        activeTiles.RemoveAt(0);
        onTilePassed?.Invoke();

        ReturnChildrenToPool(oldest);

        oldest.gameObject.SetActive(false);
        tilePools[oldest.tileType].Enqueue(oldest);
        SpawnTile();
    }

    public void PauseGame() => isRunning = false;
}
