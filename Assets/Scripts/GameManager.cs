using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    [SerializeField] private PlayerState playerState;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private BGThemeData currentTheme;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private BGTileManager bgTileManager;
    
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UIManager uimanager;

    private bool isPlaying = false;
    
    void Awake()
    {
        Application.targetFrameRate = 60;
        tileManager.onTilePassed += OnTilePassed;
    }
    void Start()
    {
        playerState.Reset();
        playerState.onDead += GameOver;
        
    }

    void Update()
    {
        if(isPlaying)
        {
            playerState.UpdateScore(Time.deltaTime);
        }
    }

    public void GameStart()
    {
        playerState.Reset();
        playerController.OnRun();

        tileManager.StartGame();
        bgTileManager.StartGame();
        cameraController.StartRun();

        uimanager.ShowIngameUI();

        if(currentTheme != null && currentTheme.bgmClip != null)
            SoundManager.Instance.PlayBGM(currentTheme.bgmClip, true);

        isPlaying = true;
    }

    void GameOver()
    {
        isPlaying = false;
        Debug.Log("게임오버");
        playerController.OnDead();
        tileManager.PauseGame();
        bgTileManager.PauseGame();
        cameraController.PlayDeadCamera();

        uimanager.ShowEndUI();
    }

    void OnTilePassed()
    {
        playerState.AddScore(gameSettings.scorePerSecond);
    }

    void OnDestroy()
    {
        playerState.onDead -= GameOver;
        tileManager.onTilePassed -= OnTilePassed;
    }
}