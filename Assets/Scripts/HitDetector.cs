
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
                playerState.AddScore(item.score);
                item.DisableItem();
            }
        }
        else if(other.CompareTag("Obstacle"))
        {
            playerState.OnHitObstacle();

        }
    }
}