using UnityEngine;

public class WarningDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        TileObject tileObject = other.GetComponent<TileObject>();
        if (tileObject != null)
        {
            tileObject.ShowOutline();
        }
    }
}