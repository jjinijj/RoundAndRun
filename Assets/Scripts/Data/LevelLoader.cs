using UnityEngine;
using System;

public class LevelLoader : MonoBehaviour
{
    [SerializeField] private TextAsset levelJson;

    public LevelData Load()
    {
        return JsonUtility.FromJson<LevelData>(levelJson.text);
    }

    public TileType ParseTileType(string tile)
    {
        if (Enum.TryParse(tile, out TileType result))
            return result;

        Debug.LogWarning($"LevelLoader: 알 수 없는 TileType '{tile}', Empty로 대체합니다.");
        return TileType.Empty;
    }
}