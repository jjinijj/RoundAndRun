using UnityEngine;

public enum ItemType { onGround, Floating }

public class Item : MonoBehaviour
{
    public int score = 10;
    [SerializeField] private Outline outline;

    public void ResetItem()
    {
        gameObject.SetActive(true);
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

    public void DisableItem()
    {
        gameObject.SetActive(false);
    }

    public void ShowOutline()
    {
        if(outline != null)
        {
            outline.enabled = true;
        }
    }
}
