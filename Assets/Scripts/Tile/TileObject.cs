using UnityEngine;

public enum TileObjectType { Obstacle, Item, Guide }

public class TileObject : MonoBehaviour
{
    [SerializeField] private TileObjectType type;
    [SerializeField] private Outline outline;

    public TileObjectType Type => type;
    public string Id { get; set; }

    void Awake()
    {
        if (outline == null)
            outline = GetComponent<Outline>();
    }

    public void ShowOutline()
    {
        if (outline != null) outline.enabled = true;
    }

    public void HideOutline()
    {
        if (outline != null) outline.enabled = false;
    }

    public void SetPosition(Vector3 position)
    {
        transform.localPosition = position;
    }

    public void Reset()
    {
        HideOutline();
    }
}
