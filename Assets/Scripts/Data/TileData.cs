[System.Serializable]
public class TileObjectData
{
    public string id;
    public float height;
    public int slot;
}

[System.Serializable]
public class TileData
{
    public string tile;
    public TileObjectData[] objects;
}
