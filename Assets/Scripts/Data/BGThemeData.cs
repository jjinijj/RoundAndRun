using UnityEngine;

[CreateAssetMenu(menuName = "Game/BGThemeData")]
public class BGThemeData : ScriptableObject
{
    public string themeName;
    public GameObject[] bgTilePrefabs;      // 배경 바닥 타일 종류
    public GameObject[] decorationPrefabs;  // 장식물 종류
    public float decorationSpawnChance;     // 장식물 생성 확률
}