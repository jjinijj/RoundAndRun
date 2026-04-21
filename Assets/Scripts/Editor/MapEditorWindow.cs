#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class MapEditorWindow : EditorWindow
{
    private BGThemeData themeData;
    private List<TileData> tiles = new();
    private string levelName = "level_01";
    private Vector2 scrollPos;

    [MenuItem("Game/Map Editor")]
    public static void Open()
    {
        GetWindow<MapEditorWindow>("Map Editor");
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

        int moveFrom = -1, moveTo = -1;

        for (int i = 0; i < tiles.Count; i++)
        {
            EditorGUILayout.BeginHorizontal();

            // 순서 변경 버튼
            GUI.enabled = i > 0;
            if (GUILayout.Button("▲", GUILayout.Width(22))) { moveFrom = i; moveTo = i - 1; }
            GUI.enabled = i < tiles.Count - 1;
            if (GUILayout.Button("▼", GUILayout.Width(22))) { moveFrom = i; moveTo = i + 1; }
            GUI.enabled = true;

            tiles[i].tile = EditorGUILayout.TextField(tiles[i].tile, GUILayout.Width(100));

            if (tiles[i].tile == "Obstacle")
                tiles[i].obstacleId = DrawIdDropdown(tiles[i].obstacleId, GetObstacleIds(), 120);
            else if (tiles[i].tile == "Item")
            {
                tiles[i].itemId = DrawIdDropdown(tiles[i].itemId, GetItemIds(), 120);
                tiles[i].height = EditorGUILayout.FloatField(tiles[i].height, GUILayout.Width(50));
            }
            else
                GUILayout.Space(175);

            if (GUILayout.Button("X", GUILayout.Width(25)))
                tiles.RemoveAt(i);

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();

        if (moveFrom >= 0)
            (tiles[moveFrom], tiles[moveTo]) = (tiles[moveTo], tiles[moveFrom]);
    }

    string DrawIdDropdown(string current, string[] options, int width)
    {
        if (options == null || options.Length == 0) return current;
        int idx = Mathf.Max(0, System.Array.IndexOf(options, current));
        idx = EditorGUILayout.Popup(idx, options, GUILayout.Width(width));
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

    void DrawFooter()
    {
        EditorGUILayout.Space();
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Empty 추가")) tiles.Add(new TileData { tile = "Empty" });
        if (GUILayout.Button("Obstacle 추가")) tiles.Add(new TileData { tile = "Obstacle" });
        if (GUILayout.Button("Item 추가")) tiles.Add(new TileData { tile = "Item" });

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
