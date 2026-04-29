
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [SerializeField] PlayerState playerState;
    void OnTriggerEnter(Collider other)
    {
        TileObject tileObject = other.GetComponent<TileObject>();
        if(tileObject == null)
            return;

        switch (tileObject.Type)
        {
            case TileObjectType.Obstacle:
                SoundManager.Instance.PlayHit();
                playerState.OnHitObstacle();
                break;
            case TileObjectType.Item:
                Item item = other.GetComponent<Item>();
                if(item != null)
                {
                    item.OnPickUp(playerState);
                    if(item.pickupClip != null)
                        SoundManager.Instance.PlayItemPickup(item.pickupClip);
                    item.DisableItem();
                }
            break;
            case TileObjectType.Guide:
            break;

            default:
            break;
        }
    }
}