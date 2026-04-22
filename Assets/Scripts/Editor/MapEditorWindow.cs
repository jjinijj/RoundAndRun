#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private BGThemeData themeData;
    private List<TileData> tiles = new();
    private string levelName = "level_01";
    private Vector2 scrollPos;
    private ReorderableList reorderableList;

    [MenuItem("Game/Map Editor")]
    public static void Open()
    {
        GetWindow<MapEditorWindow>("Map Editor");
    }

    void OnEnable()
    {
        BuildReorderableList();
    }

    void BuildReorderableList()
    {
        reorderableList = new ReorderableList(tiles, typeof(TileData), true, false, false, false)
        {
            elementHeight = EditorGUIUtility.singleLineHeight + 2,
            drawElementCallback = DrawElement
        };
    }

    void DrawElement(Rect rect, int i, bool isActive, bool isFocused)
    {
        if (i >= tiles.Count) return;

        float x = rect.x;
        float y = rect.y + 1;
        float h = EditorGUIUtility.singleLineHeight;

        EditorGUI.LabelField(new Rect(x, y, 30, h), i.ToString(), EditorStyles.miniLabel);
        x += 32;

        tiles[i].tile = EditorGUI.TextField(new Rect(x, y, 100, h), tiles[i].tile);
        x += 104;

        if (tiles[i].tile == "Obstacle")
        {
            tiles[i].obstacleId = DrawIdDropdown(new Rect(x, y, 120, h), tiles[i].obstacleId, GetObstacleIds());
        }
        else if (tiles[i].tile == "Item")
        {
            tiles[i].itemId = DrawIdDropdown(new Rect(x, y, 100, h), tiles[i].itemId, GetItemIds());
            x += 104;
            tiles[i].height = EditorGUI.FloatField(new Rect(x, y, 50, h), tiles[i].height);
        }

        if (GUI.Button(new Rect(rect.xMax - 25, y, 25, h), "X"))
            tiles.RemoveAt(i);
    }

    string DrawIdDropdown(Rect rect, string current, string[] options)
    {
        if (options == null || options.Length == 0) return current;
        int idx = Mathf.Max(0, System.Array.IndexOf(options, current));
        idx = EditorGUI.Popup(rect, idx, options);
        return options[idx];
    }

    string[] GetObstacleIds()
    {
        if (themeData == null) return new string[0];
        return System.Array.ConvertAll(themeData.obstacles, o => o.id);
    }

    string[] GetItemIds()
    {
        if (themeData == null) return new string[0];
        return System.Array.ConvertAll(themeData.items, i => i.id);
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

        if (GUILayout.Button("Empty 추가")) { tiles.Add(new TileData { tile = "Empty" }); BuildReorderableList(); }
        if (GUILayout.Button("Obstacle 추가")) { tiles.Add(new TileData { tile = "Obstacle" }); BuildReorderableList(); }
        if (GUILayout.Button("Item 추가")) { tiles.Add(new TileData { tile = "Item" }); BuildReorderableList(); }

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

        string json = File.ReadAllText(path);
        LevelData data = JsonUtility.FromJson<LevelData>(json);

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
