
using UnityEngine;

public class HitDetector : MonoBehaviour
{
    [SerializeField] PlayerState playerState;
    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if(item != null)
            {
                item.OnPickUp(playerState);
                if(item.pickupClip != null)
                    SoundManager.Instance.PlayItemPickup(item.pickupClip);
                item.DisableItem();
            }
        }
        else if(other.CompareTag("Obstacle"))
        {
            SoundManager.Instance.PlayHit();
            playerState.OnHitObstacle();
        }
    }
}