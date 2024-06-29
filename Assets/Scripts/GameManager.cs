using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static PopupPanel gameOverPanel;
    private static PopupPanel gameClearPanel;
    private static PopupPanel pausePanel;

    private static float _gameTime = 0;
    private static bool _isPauseGame = false;

    [SerializeField, Tooltip("開始時間")] private float _initTime = 0f;
    [SerializeField, Tooltip("プレイヤーのシーン共有データ")] private PlayerSharedData _sharedData;

    // getter
    public static float GameTime => _gameTime;
    public static bool IsPauseGame => _isPauseGame;

    #region --- Unityライフサイクル ---

        private void Awake()
        {
            var canvas = transform.GetChild(0);
            if (canvas.Find("GameClearPanel") == null) Debug.LogError("gameClearPanelをアタッチ or 小林まで連絡");

            gameOverPanel = canvas.Find("GameOverPanel").GetComponent<PopupPanel>();
            gameClearPanel = canvas.Find("GameClearPanel").GetComponent<PopupPanel>();
            pausePanel = canvas.Find("PausePanel").GetComponent<PopupPanel>();

        
        }

        private void Start()
        {
            GameReset();
        }

        private void Update()
        {

            if (Input.GetKeyDown(KeyCode.P) && !gameOverPanel.IsOpen)
            {
                if (!_isPauseGame) PauseGame(); else ResumeGame();
            }

            if (Input.GetKeyDown(KeyCode.R) && (pausePanel.IsOpen || gameOverPanel.IsOpen))
            {
                Retry();
            }

            GameTimerCount();
        }

    #endregion


    /// <summary>
    /// ゲーム状態のリセット
    /// </summary>
    private void GameReset()
    {
        Time.timeScale = 1;
        _gameTime = _initTime;
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
        pausePanel.SetActive(false);
        gameOverPanel.Open(() =>
        {
            Time.timeScale = 0;
            _isPauseGame = true;
        });
    }
    
    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public static void GameClear()
    {
        pausePanel.SetActive(false);
        gameClearPanel.Open(() =>
        {
            Time.timeScale = 0;
            _isPauseGame = true;
        });
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
    public static void PauseGame()
    {
        _isPauseGame = true;
        pausePanel.Open(() => {
                
            Time.timeScale = 0; 
        });
    }

    /// <summary>
    /// ゲームを再開
    /// </summary>
    public static void ResumeGame()
    {
        pausePanel.Close(() =>
        {
            Time.timeScale = 1;
            _isPauseGame = false;
        });
    }

    public static void ToTitle()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }

}
