using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    [Header("Tile Prefabs")]
    [SerializeField] private Tile emptyTilePrefab;
    [SerializeField] private Tile jumpObstacleTileShortPrefab;
    [SerializeField] private Tile jumpObstacleTileLongPrefab;
    [SerializeField] private Tile slideObstacleTilePrefab;
    [SerializeField] private Tile itemTilePrefab;
    [SerializeField] private Tile itemJumpTilePrefab;

    [Header("Settings")]
    [SerializeField] private float tileSpeed = 10f;
    [SerializeField] private int initialTileCount = 5; // 시작 시 깔아둘 타일 수

    private Dictionary<TileType, Queue<Tile>> pools = new();
    private List<Tile> activeTiles = new();
    private int sequenceIndex = 0;
    private bool isRunning = false;

    [Header("Player Reference")]
    [SerializeField] private Transform player;

    private TileType[] levelSequence;

    void Start()
    {
        levelSequence = GetComponent<LevelLoader>().Load();
        InitPool(TileType.Empty, emptyTilePrefab);
        InitPool(TileType.JumpObstacle_Short, jumpObstacleTileShortPrefab);
        InitPool(TileType.JumpObstacle_Long, jumpObstacleTileLongPrefab);
        InitPool(TileType.SlideObstacle, slideObstacleTilePrefab);
        InitPool(TileType.Item, itemTilePrefab);
        InitPool(TileType.ItemJump, itemJumpTilePrefab);

        // 시작 타일 깔기
        for (int i = 0; i < initialTileCount; i++)
            SpawnTile();
    }    

    TileType GetNextTileType()
    {
        if (sequenceIndex >= levelSequence.Length)
            sequenceIndex = 0; // 끝나면 처음부터 반복

        return levelSequence[sequenceIndex++];
    }

    void Update()
    {
        if (isRunning)
        {
            MoveTiles();
            CheckRecycle();
        }
    }

    void InitPool(TileType type, Tile prefab)
    {
        pools[type] = new Queue<Tile>();
        // 초기 풀 사이즈 3개
        for (int i = 0; i < 3; i++)
        {
            Tile t = Instantiate(prefab);
            t.gameObject.SetActive(false);
            pools[type].Enqueue(t);
        }
    }

    void SpawnTile()
    {
        TileType type = GetNextTileType();
        Tile tile = GetFromPool(type);

        // nextSpawnZ 대신 마지막 타일 기준으로 위치 계산
        float spawnZ = activeTiles.Count > 0
            ? activeTiles[activeTiles.Count - 1].transform.position.z + activeTiles[activeTiles.Count - 1].tileLength
            : 0f;

        tile.transform.position = new Vector3(0, -1f, spawnZ);
        tile.gameObject.SetActive(true);
        tile.ResetTile();
        activeTiles.Add(tile);
    }

    Tile GetFromPool(TileType type)
    {
        if (pools[type].Count > 0)
            return pools[type].Dequeue();

        // 풀 소진 시 새로 생성
        return Instantiate(GetPrefab(type));
    }

    Tile GetPrefab(TileType type) => type switch
    {
        TileType.Empty => emptyTilePrefab,
        TileType.JumpObstacle_Short => jumpObstacleTileShortPrefab,
        TileType.JumpObstacle_Long => jumpObstacleTileLongPrefab,
        TileType.SlideObstacle => slideObstacleTilePrefab,
        TileType.Item => itemTilePrefab,
        TileType.ItemJump=>itemJumpTilePrefab,
        _ => emptyTilePrefab
    };

    void MoveTiles()
    {
        foreach (var tile in activeTiles)
            tile.transform.position += Vector3.back * tileSpeed * Time.deltaTime;
    }

    void CheckRecycle()
    {
        if (activeTiles.Count == 0) return;

        Tile oldest = activeTiles[0];
        // 플레이어 뒤로 타일 하나 길이 이상 넘어가면 재활용
        if (oldest.transform.position.z < player.position.z - oldest.tileLength)
        {
            activeTiles.RemoveAt(0);
            oldest.gameObject.SetActive(false);
            pools[oldest.tileType].Enqueue(oldest);
            SpawnTile();
        }
    }

    public void StartGame()
    {
        isRunning = true;
    }

    public void PauseGame()
    {
        isRunning = false;
    }
}
