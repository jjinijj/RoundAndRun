using System;
using UnityEngine;

[Serializable]
public class ObstacleData
{
    public string id;
    public string displayName;
    public GameObject prefab;
}

[CreateAssetMenu(menuName = "Game/BGThemeData")]
public class BGThemeData : ScriptableObject
{
    public string themeName;
    public AudioClip bgmClip;
    public GameObject[] bgTilePrefabs;
    public GameObject[] decorationPrefabs;
    public float decorationSpawnChance;
    public ObjectData[] objects;

    public ObjectData GetObject(string id) => System.Array.Find(objects, o => o.id == id);

}