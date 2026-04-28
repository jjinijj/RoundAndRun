using UnityEngine;

public class ScoreItem : Item
{
    public int score = 10;

    public override void OnPickUp(PlayerState playerState)
    {
        playerState.AddScore(score);
    }
}
