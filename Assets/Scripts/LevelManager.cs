using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private BGTileManager bgTileManager;
    [SerializeField] private LevelLoader levelLoader;

    private int currentLevelIndex = 0;

    void Start()
    {
        tileManager.onLevelComplete += OnLevelComplete;
    }

    public void LoadCurrentLevel()
    {
        LevelEntry entry = levelDatabase.levels[currentLevelIndex];
        LevelData levelData = levelLoader.LoadByName(entry.levelJsonName);

        tileManager.SetTheme(entry.themeData);
        tileManager.SetLevel(levelData);
        bgTileManager.SetTheme(entry.themeData);
        SoundManager.Instance.PlayBGM(entry.themeData.bgmClip, true);
    }

    void OnLevelComplete()
    {
        if (currentLevelIndex + 1 < levelDatabase.levels.Length)
        {
            currentLevelIndex++;
            LoadCurrentLevel();
        }
        else
        {
            Debug.Log("모든 레벨 완주");
        }
    }

    public void Reset()
    {
        currentLevelIndex = 0;
    }

    void OnDestroy()
    {
        tileManager.onLevelComplete -= OnLevelComplete;
    }
}
