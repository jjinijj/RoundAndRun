#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BGThemeData))]
public class BGThemeDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BGThemeData data = (BGThemeData)target;

        if (GUILayout.Button("ID 자동 생성"))
        {
            for (int i = 0; i < data.obstacles.Length; i++)
                data.obstacles[i].id = $"obstacle_{data.themeName.ToLower()}_{(i + 1):D3}";

            for (int i = 0; i < data.items.Length; i++)
                data.items[i].id = $"item_{data.themeName.ToLower()}_{(i + 1):D3}";

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
