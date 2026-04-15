using UnityEngine;
using System;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private TextAsset levelJson;

    public TileType[] Load()
    {
        LevelData data = JsonUtility.FromJson<LevelData>(levelJson.text);
        TileType[] result = new TileType[data.tiles.Length];

        for (int i = 0; i < data.tiles.Length; i++)
            result[i] = Enum.Parse<TileType>(data.tiles[i]);

        return result;
    }
}