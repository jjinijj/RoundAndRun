using System;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private ItemDatabase itemDatabase;
    [SerializeField] private int initialTileCount = 5;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    public Action onTilePassed;
    public Action onLevelComplete;
    public Action onStaleTilesCleared;

    private BGThemeData themeData;
    private Queue<Tile> tilePool = new();
    private Dictionary<string, Queue<GameObject>> objectPools = new();
    private Dictionary<string, Queue<GameObject>> itemPools = new();
    private List<Tile> activeTiles = new();
    private HashSet<Tile> staleTiles = new();
    private int sequenceIndex = 0;
    private bool isRunning = false;
    private LevelData levelData;

    void Start()
    {
        InitItemPools();
    }

    void InitTilePool()
    {
        tilePool.Clear();
        for (int i = 0; i < 3; i++)
        {
            Tile t = Instantiate(themeData.tilePrefab);
            t.gameObject.SetActive(false);
            tilePool.Enqueue(t);
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

    public void SetTheme(BGThemeData data)
    {
        foreach (var tile in activeTiles)
            staleTiles.Add(tile);

        foreach (var t in tilePool) Destroy(t.gameObject);
        tilePool.Clear();

        foreach (var queue in objectPools.Values)
            while (queue.Count > 0) Destroy(queue.Dequeue());
        objectPools.Clear();

        themeData = data;
        InitTilePool();
        InitObjectPools();

        if (staleTiles.Count == 0)
            onStaleTilesCleared?.Invoke();
    }

    public void SetLevel(LevelData data)
    {
        levelData = data;
        sequenceIndex = 0;
    }

    public void StartGame()
    {
        if (levelData == null)
        {
            Debug.LogError("TileManager: levelData가 없습니다. SetLevel()을 먼저 호출하세요.");
            return;
        }
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
            tilePool.Enqueue(tile);
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
        {
            sequenceIndex = 0;
            onLevelComplete?.Invoke();
        }
        return levelData.tiles[sequenceIndex++];
    }

    void SpawnTile()
    {
        TileData tileData = GetNextTileData();
        Tile tile = GetFromTilePool();

        float spawnZ = activeTiles.Count > 0
            ? activeTiles[activeTiles.Count - 1].transform.position.z + gameSettings.tileLength
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

    Tile GetFromTilePool()
    {
        if (tilePool.Count > 0) return tilePool.Dequeue();
        return Instantiate(themeData.tilePrefab);
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
        if (oldest.transform.position.z >= player.position.z - gameSettings.tileLength) return;

        activeTiles.RemoveAt(0);
        onTilePassed?.Invoke();

        if (staleTiles.Remove(oldest))
        {
            foreach (var child in oldest.DetachAll()) Destroy(child);
            Destroy(oldest.gameObject);
            if (staleTiles.Count == 0)
                onStaleTilesCleared?.Invoke();
        }
        else
        {
            ReturnChildrenToPool(oldest);
            oldest.gameObject.SetActive(false);
            tilePool.Enqueue(oldest);
        }
        SpawnTile();
    }

    public void PauseGame() => isRunning = false;
}
