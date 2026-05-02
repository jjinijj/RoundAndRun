using UnityEngine;
using System;

public class LevelLoader : MonoBehaviour
{
    private const string LevelPath = "Levels/";

    public LevelData LoadByName(string levelJsonName)
    {
        TextAsset asset = Resources.Load<TextAsset>(LevelPath + levelJsonName);
        if (asset == null)
        {
            Debug.LogError($"LevelLoader: '{levelJsonName}' 파일을 찾을 수 없습니다. (Resources/Levels/ 확인)");
            return null;
        }
        return JsonUtility.FromJson<LevelData>(asset.text);
    }

    public TileType ParseTileType(string tile)
    {
        if (Enum.TryParse(tile, out TileType result))
            return result;

        Debug.LogWarning($"LevelLoader: 알 수 없는 TileType '{tile}', Empty로 대체합니다.");
        return TileType.Empty;
    }
}