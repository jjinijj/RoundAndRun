using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerState")]
public class PlayerState : ScriptableObject
{
    [SerializeField] GameSettings gameSettings;
    [SerializeField] private int score;
    public int Score => score;

    public int life;
    public bool isDead;

    public Action onDead;
    public Action<int, int> onChangeLife;
    public Action onDecreaseLife;
    public Action onIncreaseLife;
    
    // PlayerState
    public void UpdateScore(float deltaTime)
    {
        if (isDead) 
            return;
            
        score += Mathf.FloorToInt(gameSettings.scorePerSecond * deltaTime);
    }

    public void AddScore(int amount)
    {
        if (isDead) return;
        score += amount;
    }

    public void OnHitObstacle()
    {
        life--;
        onChangeLife?.Invoke(life, gameSettings.maxLife);
        onDecreaseLife?.Invoke();
        if (life <= 0)
        {
            isDead = true;
            onDead?.Invoke();
        }
    }

    public void Reset()
    {
        score = 0;
        life = gameSettings.maxLife;
        isDead = false;
        onChangeLife?.Invoke(life, life);
    }
}