using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public ObjectData[] items;

    public ObjectData GetItem(string id) =>
        Array.Find(items, i => i.id == id);
}
