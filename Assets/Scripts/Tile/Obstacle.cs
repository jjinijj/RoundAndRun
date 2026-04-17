using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Outline outline;
    void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void ShowOutline()
    {
        if(outline != null)
        {
            outline.enabled = true;
        }
    }

    public void HideOutline()
    {
        if(outline != null)
        {
            outline.enabled = false;
        }
    }
}