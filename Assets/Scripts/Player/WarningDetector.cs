using UnityEngine;

public class WarningDetector : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Outline outline = other.GetComponent<Outline>();
        if (outline)
        {
            outline.enabled = true;
        }
    }
}