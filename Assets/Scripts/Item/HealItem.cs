using UnityEngine;

public class HealItem : Item
{
    public int healAmount = 1;

    public override void OnPickUp(PlayerState playerState)
    {
        playerState.Heal(healAmount);
    }
}
