using System.Data;
using UnityEngine;
using UnityEngine.Assertions.Must;

public enum ItemType {onGround, Floating}
public class Item : MonoBehaviour
{
    public int score = 10;
    public void OnTriggerEnter(Collider other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if(player != null)
        {
            DisableItem();
        }
    }

    public void OnCollisionEnter(Collision collision)
    {
        Debug.Log("hit : " + collision.gameObject.name);
    }

    public void ResetItem()
    {
        gameObject.SetActive(true);
    }

    public void DisableItem()
    {
        gameObject.SetActive(false);
    }

}