using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static Canvas gameOverCanvas;
    private static Canvas pauseCanvas;

    private static float _gameTime = 0;
    private static bool _isPauseGame = false;

    [SerializeField, Tooltip("プレイヤーのシーン共有データ")] private PlayerSharedData _sharedData;

    // getter
    public static float GameTime => _gameTime;
    public static bool IsPauseGame => _isPauseGame;

    private void Awake()
    {
        gameOverCanvas = transform.Find("GameOverCanvas").GetComponent<Canvas>();
        pauseCanvas = transform.Find("PauseCanvas").GetComponent<Canvas>();
    }

    private void Start()
    {
        GameReset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && !gameOverCanvas.enabled)
        {
            _isPauseGame = !_isPauseGame;
            if (_isPauseGame) PauseGame(); else ResumeGame();
        }

        if (Input.GetKeyDown(KeyCode.R) && (pauseCanvas.enabled || gameOverCanvas.enabled))
        {
            Retry();
        }

        GameTimerCount();
    }

    /// <summary>
    /// ゲーム状態のリセット
    /// </summary>
    private void GameReset()
    {
        _gameTime = 0;
        _sharedData.Reset();
        _isPauseGame = false;
        ResumeGame();
    }

    /// <summary>
    /// ゲーム経過時間を計測
    /// </summary>
    private void GameTimerCount()
    {
        _gameTime += Time.deltaTime;
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public static void GameOver()
    {
        PauseGame(false);
        gameOverCanvas.enabled = true;
    }



    /// <summary>
    /// ゲームをはじめから読み込む
    /// </summary>
    public static void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// ゲームを一時停止
    /// </summary>
    public static void PauseGame(bool enabelCanvas = true)
    {
        Time.timeScale = 0;
        pauseCanvas.enabled = enabelCanvas;
    }

    /// <summary>
    /// ゲームを再開
    /// </summary>
    public static void ResumeGame()
    {
        Time.timeScale = 1;
        pauseCanvas.enabled = false;
    }

    public static void ToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }
}
