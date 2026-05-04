using UnityEngine;

[CreateAssetMenu(menuName = "Game/GameSettings")]
public class GameSettings : ScriptableObject
{
    public float tileLength = 2f;
    public int maxLife = 5;
    public int scorePerSecond = 1;

    [System.NonSerialized] public float currentSpeed;
}
