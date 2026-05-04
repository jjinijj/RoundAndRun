using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private LevelDatabase levelDatabase;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private BGTileManager bgTileManager;
    [SerializeField] private LevelLoader levelLoader;
    [SerializeField] private GameSettings gameSettings;

    private int currentLevelIndex = 0;
    private AudioClip pendingBGM;

    private float levelStartSpeed;
    private float levelMaxSpeed;
    private float acceleration;
    private float distanceTraveled;
    private bool isLastLevelRepeating;
    private bool isRunning;

    void Start()
    {
        tileManager.onLevelComplete += OnLevelComplete;
        tileManager.onStaleTilesCleared += OnStaleTilesCleared;
    }

    void Update()
    {
        if (!isRunning) return;

        if (isLastLevelRepeating) return;

        distanceTraveled += gameSettings.currentSpeed * Time.deltaTime;
        gameSettings.currentSpeed = Mathf.Min(
            levelStartSpeed + acceleration * distanceTraveled,
            levelMaxSpeed
        );
    }

    public void LoadCurrentLevel()
    {
        LevelEntry entry = levelDatabase.levels[currentLevelIndex];
        LevelData levelData = levelLoader.LoadByName(entry.levelJsonName);

        pendingBGM = entry.themeData.bgmClip;

        SetupSpeed(levelData);

        tileManager.SetTheme(entry.themeData);
        tileManager.SetLevel(levelData);
        bgTileManager.SetTheme(entry.themeData);
    }

    void SetupSpeed(LevelData levelData)
    {
        bool isFirst = !isRunning;
        bool isLastLevel = currentLevelIndex == levelDatabase.levels.Length - 1;

        levelStartSpeed = isFirst ? levelData.initialSpeed : gameSettings.currentSpeed;
        levelMaxSpeed = levelData.maxSpeed;

        if (isLastLevelRepeating)
        {
            // 마지막 레벨 반복: 현재 속도(= 이전 최종 속도) 유지, 가속 없음
            acceleration = 0f;
            return;
        }

        isLastLevelRepeating = isLastLevel && !isFirst;

        float totalDistance = levelData.tiles.Length * gameSettings.tileLength;
        acceleration = totalDistance > 0f
            ? (levelMaxSpeed - levelStartSpeed) / totalDistance
            : 0f;

        distanceTraveled = 0f;

        if (isFirst)
            gameSettings.currentSpeed = levelStartSpeed;
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
            isLastLevelRepeating = true;
            LoadCurrentLevel();
        }
    }

    public void StartGame() => isRunning = true;
    public void PauseGame() => isRunning = false;
    public void ResumeGame() => isRunning = true;

    public void Reset()
    {
        currentLevelIndex = 0;
        isLastLevelRepeating = false;
        isRunning = false;
    }

    void OnDestroy()
    {
        tileManager.onLevelComplete -= OnLevelComplete;
        tileManager.onStaleTilesCleared -= OnStaleTilesCleared;
    }
}
