using System;
using UnityEngine;

[Serializable]
public class ObstacleData
{
    public string id;
    public string displayName;
    public GameObject prefab;
}

[Serializable]
public class ItemData
{
    public string id;
    public string displayName;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "Game/BGThemeData")]
public class BGThemeData : ScriptableObject
{
    public string themeName;
    public GameObject[] bgTilePrefabs;
    public GameObject[] decorationPrefabs;
    public float decorationSpawnChance;
    public ObstacleData[] obstacles;
    public ItemData[] items;

    public ObstacleData GetObstacle(string id) => System.Array.Find(obstacles, o => o.id == id);

    public ItemData GetItem(string id) => System.Array.Find(items, i => i.id == id);
}