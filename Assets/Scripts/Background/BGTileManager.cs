using System.Collections.Generic;
using UnityEngine;

public class BGTileManager : MonoBehaviour
{
    [Header("Theme")]
    [SerializeField] private BGThemeData themeData;

    [Header("Settings")]
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private float tileLength = 10f;
    [SerializeField] private float tileY = 0f;
    [SerializeField] private float xOffset = 5f;
    [SerializeField] private int initialTileCount = 8;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    private List<Queue<BGTile>> tilePools = new();
    private List<Queue<GameObject>> decorationPools = new();
    private List<BGTile> activeTilesLeft = new();
    private List<BGTile> activeTilesRight = new();
    private HashSet<BGTile> staleTiles = new();
    private bool isRunning = false;

    void Start()
    {
        InitPools();
        for (int i = 0; i < initialTileCount; i++)
        {
            SpawnTile(activeTilesLeft, -xOffset);
            SpawnTile(activeTilesRight, xOffset);
        }
    }

    void InitPools()
    {
        for (int i = 0; i < themeData.bgTilePrefabs.Length; i++)
        {
            var q = new Queue<BGTile>();
            for (int j = 0; j < 3; j++)
                q.Enqueue(CreateTile(i));
            tilePools.Add(q);
        }

        if (themeData.decorationPrefabs == null) return;
        for (int i = 0; i < themeData.decorationPrefabs.Length; i++)
        {
            var q = new Queue<GameObject>();
            for (int j = 0; j < 3; j++)
                q.Enqueue(CreateDecoration(i));
            decorationPools.Add(q);
        }
    }

    BGTile CreateTile(int index)
    {
        var go = Instantiate(themeData.bgTilePrefabs[index], transform);
        go.SetActive(false);
        var bgTile = go.AddComponent<BGTile>();
        bgTile.prefabIndex = index;
        return bgTile;
    }

    GameObject CreateDecoration(int index)
    {
        var go = Instantiate(themeData.decorationPrefabs[index], transform);
        go.SetActive(false);
        return go;
    }

    void SpawnTile(List<BGTile> activeTiles, float x)
    {
        int tileIdx = Random.Range(0, themeData.bgTilePrefabs.Length);
        BGTile tile = GetFromTilePool(tileIdx);

        float spawnZ = activeTiles.Count > 0
            ? activeTiles[activeTiles.Count - 1].transform.position.z + tileLength
            : 0f;

        tile.transform.position = new Vector3(x, tileY, spawnZ);
        tile.gameObject.SetActive(true);

        TryAttachDecoration(tile);
        activeTiles.Add(tile);
    }

    void TryAttachDecoration(BGTile tile)
    {
        if (themeData.decorationPrefabs == null || themeData.decorationPrefabs.Length == 0) return;
        if (Random.value >= themeData.decorationSpawnChance) return;

        int decIdx = Random.Range(0, themeData.decorationPrefabs.Length);
        GameObject dec = GetFromDecorationPool(decIdx);

        dec.transform.SetParent(tile.transform);
        dec.transform.localPosition = Vector3.zero;
        dec.SetActive(true);

        tile.decoration = dec;
        tile.decorationIndex = decIdx;
    }

    BGTile GetFromTilePool(int index)
    {
        if (tilePools[index].Count > 0)
            return tilePools[index].Dequeue();
        return CreateTile(index);
    }

    GameObject GetFromDecorationPool(int index)
    {
        if (decorationPools[index].Count > 0)
            return decorationPools[index].Dequeue();
        return CreateDecoration(index);
    }

    void Update()
    {
        if (!isRunning) return;
        MoveTiles();
        CheckRecycle();
    }

    void MoveTiles()
    {
        foreach (var tile in activeTilesLeft)
            tile.transform.position += Vector3.back * gameSettings.tileSpeed * Time.deltaTime;
        foreach (var tile in activeTilesRight)
            tile.transform.position += Vector3.back * gameSettings.tileSpeed * Time.deltaTime;
    }

    void CheckRecycle()
    {
        CheckRecycleLane(activeTilesLeft, -xOffset);
        CheckRecycleLane(activeTilesRight, xOffset);
    }

    void CheckRecycleLane(List<BGTile> activeTiles, float x)
    {
        if (activeTiles.Count == 0) return;

        BGTile oldest = activeTiles[0];
        if (oldest.transform.position.z >= player.position.z - tileLength) return;

        activeTiles.RemoveAt(0);

        if (staleTiles.Remove(oldest))
        {
            if (oldest.decoration != null) Destroy(oldest.decoration);
            Destroy(oldest.gameObject);
        }
        else
        {
            RecycleTile(oldest);
        }

        SpawnTile(activeTiles, x);
    }

    void RecycleTile(BGTile tile)
    {
        if (tile.decoration != null)
        {
            tile.decoration.transform.SetParent(transform);
            tile.decoration.SetActive(false);
            decorationPools[tile.decorationIndex].Enqueue(tile.decoration);
        }

        tile.ResetState();
        tile.gameObject.SetActive(false);
        tilePools[tile.prefabIndex].Enqueue(tile);
    }

    public void SetTheme(BGThemeData data)
    {
        foreach (var tile in activeTilesLeft) staleTiles.Add(tile);
        foreach (var tile in activeTilesRight) staleTiles.Add(tile);

        foreach (var pool in tilePools)
            while (pool.Count > 0) Destroy(pool.Dequeue().gameObject);
        tilePools.Clear();

        foreach (var pool in decorationPools)
            while (pool.Count > 0) Destroy(pool.Dequeue());
        decorationPools.Clear();

        themeData = data;
        InitPools();
    }

    public void StartGame() => isRunning = true;
    public void PauseGame() => isRunning = false;
}
