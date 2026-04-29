#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private BGThemeData themeData;
    private ItemDatabase itemDatabase;
    private List<TileData> tiles = new();
    private string levelName = "level_01";
    private Vector2 scrollPos;
    private ReorderableList reorderableList;

    private const float LINE_H = 18f;
    private const float LINE_SPACING = 2f;

    [MenuItem("Game/Map Editor")]
    public static void Open() => GetWindow<MapEditorWindow>("Map Editor");

    void OnEnable() => BuildReorderableList();

    void BuildReorderableList()
    {
        reorderableList = new ReorderableList(tiles, typeof(TileData), true, false, false, false)
        {
            drawElementCallback = DrawElement,
            elementHeightCallback = GetElementHeight
        };
    }

    float GetElementHeight(int i)
    {
        if (i >= tiles.Count) return LINE_H;
        int objCount = tiles[i].objects != null ? tiles[i].objects.Length : 0;
        return (LINE_H + LINE_SPACING) * (1 + objCount);
    }

    void DrawElement(Rect rect, int i, bool isActive, bool isFocused)
    {
        if (i >= tiles.Count) return;

        float h = LINE_H;
        float y = rect.y + 1;

        // 타일 행
        EditorGUI.LabelField(new Rect(rect.x, y, 30, h), i.ToString(), EditorStyles.miniLabel);

        EditorGUI.LabelField(new Rect(rect.x + 32, y, 60, h), "Empty", EditorStyles.miniLabel);

        if (GUI.Button(new Rect(rect.x + 96, y, 60, h), "+ Object"))
            AddObject(i);

        if (GUI.Button(new Rect(rect.xMax - 25, y, 25, h), "X"))
        {
            tiles.RemoveAt(i);
            BuildReorderableList();
            return;
        }

        // 오브젝트 행
        if (tiles[i].objects == null) return;
        string[] allIds = GetAllIds();

        for (int j = 0; j < tiles[i].objects.Length; j++)
        {
            y += LINE_H + LINE_SPACING;
            float x = rect.x + 20;

            tiles[i].objects[j].id = DrawIdDropdown(new Rect(x, y, 140, h), tiles[i].objects[j].id, allIds);
            x += 144;

            EditorGUI.LabelField(new Rect(x, y, 30, h), "H", EditorStyles.miniLabel);
            x += 14;
            float newHeight = EditorGUI.FloatField(new Rect(x, y, 40, h), tiles[i].objects[j].height);
            tiles[i].objects[j].height = Mathf.Round(newHeight * 10f) / 10f;
            x += 44;

            EditorGUI.LabelField(new Rect(x, y, 30, h), "S", EditorStyles.miniLabel);
            x += 14;
            tiles[i].objects[j].slot = EditorGUI.IntField(new Rect(x, y, 30, h), tiles[i].objects[j].slot);
            x += 34;

            if (GUI.Button(new Rect(x, y, 25, h), "X"))
            {
                var list = new List<TileObjectData>(tiles[i].objects);
                list.RemoveAt(j);
                tiles[i].objects = list.ToArray();
                BuildReorderableList();
                return;
            }
        }
    }

    void AddObject(int tileIndex)
    {
        var list = new List<TileObjectData>(tiles[tileIndex].objects ?? new TileObjectData[0]);
        list.Add(new TileObjectData { id = "", height = 0f, slot = 1 });
        tiles[tileIndex].objects = list.ToArray();
        BuildReorderableList();
    }

    string DrawIdDropdown(Rect rect, string current, string[] options)
    {
        if (options == null || options.Length == 0)
        {
            EditorGUI.TextField(rect, current);
            return current;
        }
        int idx = Mathf.Max(0, System.Array.IndexOf(options, current));
        idx = EditorGUI.Popup(rect, idx, options);
        return options[idx];
    }

    string[] GetAllIds()
    {
        var ids = new List<string>();
        if (themeData != null && themeData.objects != null)
            foreach (var o in themeData.objects) ids.Add(o.id);
        if (itemDatabase != null && itemDatabase.items != null)
            foreach (var o in itemDatabase.items) ids.Add(o.id);
        return ids.ToArray();
    }

    void OnGUI()
    {
        DrawHeader();
        DrawTileList();
        DrawFooter();
    }

    void DrawHeader()
    {
        GUILayout.Label("Map Editor", EditorStyles.boldLabel);
        themeData = (BGThemeData)EditorGUILayout.ObjectField("Theme", themeData, typeof(BGThemeData), false);
        itemDatabase = (ItemDatabase)EditorGUILayout.ObjectField("Item Database", itemDatabase, typeof(ItemDatabase), false);
        levelName = EditorGUILayout.TextField("Level Name", levelName);
        EditorGUILayout.Space();
    }

    void DrawTileList()
    {
        scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
        reorderableList.DoLayoutList();
        EditorGUILayout.EndScrollView();
    }

    void DrawFooter()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("타일 추가"))
        {
            tiles.Add(new TileData { tile = "Empty", objects = new TileObjectData[0] });
            BuildReorderableList();
        }

        EditorGUILayout.EndHorizontal();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("불러오기")) LoadJson();
        if (GUILayout.Button("저장")) SaveJson();
        EditorGUILayout.EndHorizontal();
    }

    void LoadJson()
    {
        if (string.IsNullOrEmpty(levelName))
        {
            EditorUtility.DisplayDialog("오류", "레벨 이름을 입력해주세요.", "확인");
            return;
        }

        string path = $"Assets/Resources/Levels/{levelName}.json";
        if (!File.Exists(path))
        {
            EditorUtility.DisplayDialog("오류", $"{levelName}.json 파일을 찾을 수 없습니다.", "확인");
            return;
        }

        LevelData data = JsonUtility.FromJson<LevelData>(File.ReadAllText(path));
        tiles = new List<TileData>(data.tiles);
        BuildReorderableList();

        if (!string.IsNullOrEmpty(data.theme))
        {
            string[] guids = AssetDatabase.FindAssets($"t:BGThemeData {data.theme}");
            if (guids.Length > 0)
                themeData = AssetDatabase.LoadAssetAtPath<BGThemeData>(AssetDatabase.GUIDToAssetPath(guids[0]));
        }

        Debug.Log($"불러오기 완료: {path}");
    }

    void SaveJson()
    {
        if (string.IsNullOrEmpty(levelName))
        {
            EditorUtility.DisplayDialog("오류", "레벨 이름을 입력해주세요.", "확인");
            return;
        }

        if (themeData == null)
        {
            EditorUtility.DisplayDialog("오류", "테마를 선택해주세요.", "확인");
            return;
        }

        string path = $"Assets/Resources/Levels/{levelName}.json";

        if (File.Exists(path))
        {
            bool overwrite = EditorUtility.DisplayDialog(
                "파일 덮어쓰기",
                $"{levelName}.json 이미 존재합니다. 덮어쓸까요?",
                "덮어쓰기",
                "취소"
            );
            if (!overwrite) return;
        }

        LevelData data = new LevelData
        {
            theme = themeData.themeName,
            tiles = tiles.ToArray()
        };

        string json = JsonUtility.ToJson(data, true);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, json);
        AssetDatabase.Refresh();
        Debug.Log($"저장 완료: {path}");
    }
}
#endif
