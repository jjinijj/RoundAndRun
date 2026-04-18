using UnityEngine;

public class BGTile : MonoBehaviour
{
    public int prefabIndex;
    public GameObject decoration;
    public int decorationIndex = -1;

    public void ResetState()
    {
        prefabIndex = 0;
        decoration = null;
        decorationIndex = -1;
    }
}
