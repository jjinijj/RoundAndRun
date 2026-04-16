using UnityEngine;

public enum ItemType { onGround, Floating }

public class Item : MonoBehaviour
{
    public int score = 10;

    public void ResetItem()
    {
        gameObject.SetActive(true);
    }

    public void DisableItem()
    {
        gameObject.SetActive(false);
    }
}
