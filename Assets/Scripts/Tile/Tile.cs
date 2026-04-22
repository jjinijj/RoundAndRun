using UnityEngine;

public enum TileType { Empty, Obstacle, Item }

public class Tile : MonoBehaviour
{
    public TileType tileType;
    public float tileLength = 10f;

    public GameObject dynamicChild { get; private set; }

    public void AttachChild(GameObject obj, Vector3 localPos)
    {
        dynamicChild = obj;
        dynamicChild.transform.SetParent(transform);
        dynamicChild.transform.localPosition = localPos;
        dynamicChild.SetActive(true);

        var outline = obj.GetComponentInChildren<Outline>();
        if (outline != null) outline.enabled = false;
    }

    public GameObject DetachChild()
    {
        if (dynamicChild == null) return null;
        dynamicChild.transform.SetParent(null);
        dynamicChild.SetActive(false);
        var obj = dynamicChild;
        dynamicChild = null;
        return obj;
    }

    public void ResetTile()
    {
        // Destroy 안 함, DetachChild는 TileManager에서 호출
    }
}