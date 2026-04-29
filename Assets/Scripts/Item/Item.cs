using UnityEngine;

public class Item : MonoBehaviour
{
    public AudioClip pickupClip;

    public virtual void OnPickUp(PlayerState playerState) { }

    public void ResetItem()
    {
        gameObject.SetActive(true);
    }

    public void DisableItem()
    {
        gameObject.SetActive(false);
    }
}
