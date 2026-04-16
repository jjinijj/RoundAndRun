using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/PlayerState")]
public class PlayerState : ScriptableObject
{
    public int score;
    public int life;
    public bool isDead;

    public Action onDead;
    public Action<int> onChangeScore;
    public Action<int> onChangeLife;
    

    public void AddScore(int amount)
    {
        if (isDead) return;
        score += amount;
        onChangeScore?.Invoke(score);
    }

    public void OnHitObstacle()
    {
        life--;
        onChangeLife?.Invoke(life);
        if (life <= 0)
        {
            isDead = true;
            onDead?.Invoke();
        }
    }

    public void Reset()
    {
        score = 0;
        life = 5;
        isDead = false;
        onChangeLife?.Invoke(life);
        onChangeScore?.Invoke(score);
    }
}