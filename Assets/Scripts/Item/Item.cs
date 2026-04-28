using UnityEngine;

public enum ItemType { onGround, Floating }

public class Item : MonoBehaviour
{
    public AudioClip pickupClip;
    [SerializeField] private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public virtual void OnPickUp(PlayerState playerState) { }

    public void ResetItem()
    {
        gameObject.SetActive(true);
        if (outline != null)
            outline.enabled = false;
    }

    public void DisableItem()
    {
        gameObject.SetActive(false);
    }

    public void ShowOutline()
    {
        if (outline != null)
            outline.enabled = true;
    }
}