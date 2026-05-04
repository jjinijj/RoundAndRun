using System.Collections.Generic;
using UnityEngine;

public enum TileType { Empty, Obstacle, Item }

public class Tile : MonoBehaviour
{
    public TileType tileType;

    private readonly List<GameObject> dynamicChildren = new();

    public void AttachChild(GameObject obj, Vector3 localPos)
    {
        obj.transform.SetParent(transform);
        obj.transform.localPosition = localPos;
        obj.SetActive(true);
        if (obj.TryGetComponent(out TileObject tileObject)) tileObject.Reset();
        dynamicChildren.Add(obj);
    }

    public List<GameObject> DetachAll()
    {
        var detached = new List<GameObject>(dynamicChildren);
        foreach (var obj in dynamicChildren)
        {
            obj.GetComponent<TileObject>()?.Reset();
            obj.transform.SetParent(null);
            obj.SetActive(false);
        }
        dynamicChildren.Clear();
        return detached;
    }

    public void ResetTile() { }
}
