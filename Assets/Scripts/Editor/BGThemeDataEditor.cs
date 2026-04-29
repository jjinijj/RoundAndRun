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
            if (data.objects != null)
            {
                for (int i = 0; i < data.objects.Length; i++)
                    data.objects[i].id = $"object_{data.themeName.ToLower()}_{i + 1:D3}";
            }

            EditorUtility.SetDirty(data);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif
