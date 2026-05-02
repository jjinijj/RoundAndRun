using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/LevelDatabase")]
public class LevelDatabase : ScriptableObject
{
    public LevelEntry[] levels;
}

[Serializable]
public class LevelEntry
{
    public string levelJsonName;
    public BGThemeData themeData;
}
