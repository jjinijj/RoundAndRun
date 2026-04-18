using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{

    [SerializeField] private PlayerState playerState;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TileManager tileManager;
    [SerializeField] private BGTileManager bgTileManager;
    
    [SerializeField] private CameraController cameraController;
    [SerializeField] private UIManager uimanager;
    // GameManager
    void Start()
    {
        playerState.Reset();
        playerState.onDead += GameOver;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            GameStart();
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
    }

    void GameOver()
    {
        Debug.Log("게임오버");
        playerController.OnDead();
        tileManager.PauseGame();
        bgTileManager.PauseGame();
        cameraController.PlayDeadCamera();

        uimanager.ShowEndUI();
    }

    void OnDestroy()
    {
        playerState.onDead -= GameOver;
    }
}