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
        {
            if (!Enum.TryParse(data.tiles[i], out result[i]))
            {
                Debug.LogWarning($"LevelLoader: 알 수 없는 TileType '{data.tiles[i]}', Empty로 대체합니다.");
                result[i] = TileType.Empty;
            }
        }

        return result;
    }
}