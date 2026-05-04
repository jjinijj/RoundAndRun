using UnityEngine;
using UnityEngine.Android;

[CreateAssetMenu(menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float tileSpeed = 10f;
    public float tileLength = 2f;

    public int maxLife = 5;

    // GameSettings
    public int scorePerSecond = 1; // 레벨별로 조정
}
