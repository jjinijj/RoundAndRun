using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private BGTileManager bgTileManager;
    [SerializeField] private LevelLoader levelLoader;

    private int currentLevelIndex = 0;
    private AudioClip pendingBGM;

    void Start()
    {
        tileManager.onLevelComplete += OnLevelComplete;
        tileManager.onStaleTilesCleared += OnStaleTilesCleared;
    }

    public void LoadCurrentLevel()
    {
        LevelEntry entry = levelDatabase.levels[currentLevelIndex];
        LevelData levelData = levelLoader.LoadByName(entry.levelJsonName);

        pendingBGM = entry.themeData.bgmClip;

        tileManager.SetTheme(entry.themeData);
        tileManager.SetLevel(levelData);
        bgTileManager.SetTheme(entry.themeData);
    }

    void OnStaleTilesCleared()
    {
        if (pendingBGM != null)
            SoundManager.Instance.PlayBGM(pendingBGM, true);
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
            LoadCurrentLevel();
        }
    }

    public void Reset()
    {
        currentLevelIndex = 0;
    }

    void OnDestroy()
    {
        tileManager.onLevelComplete -= OnLevelComplete;
        tileManager.onStaleTilesCleared -= OnStaleTilesCleared;
    }
}
