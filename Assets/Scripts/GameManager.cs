using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PopupPanel gameOverPanel;
    [SerializeField] private PopupPanel gameResultPanel;
    [SerializeField] private PopupPanel pausePanel;

    private float _gameTime = 0;
    private bool _isPauseGame = false;

    [SerializeField, Tooltip("開始時間")] private float _initTime = 0f;
    [SerializeField, Tooltip("プレイヤーのシーン共有データ")] private PlayerSharedData _sharedData;

    // getter
    public float GameTime => _gameTime;
    public bool IsPauseGame => _isPauseGame;

    #region --- Unityライフサイクル ---

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
        //第一ステージのみ
        if (StageManager.IsFirstStage())
        {
            _sharedData.Reset();
        }

        //UI
        gameResultPanel.SetActive(false);
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);

        _gameTime = _initTime + _sharedData.GetElapsedTime;
        Time.timeScale = 1;
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
    public void GameOver()
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
    public void GameClear()
    {
        pausePanel.SetActive(false);
        if (StageManager.ExistsNextStage())
        {
            _sharedData.AddElapsedTime(_gameTime);
            StageManager.GoNextStage();
        }
        else
        {
            gameResultPanel.Open(() =>
            {
                Time.timeScale = 0;
                _isPauseGame = true;
            });
        }
        
    }



    /// <summary>
    /// ゲームをはじめから読み込む
    /// </summary>
    public void Retry()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StageManager.GoStageAt(0);
    }

    /// <summary>
    /// ゲームを一時停止
    /// </summary>
    public void PauseGame()
    {
        _isPauseGame = true;
        pausePanel.Open(() => {
                
            Time.timeScale = 0; 
        });
    }

    /// <summary>
    /// ゲームを再開
    /// </summary>
    public void ResumeGame()
    {
        pausePanel.Close(() =>
        {
            Time.timeScale = 1;
            _isPauseGame = false;
        });
    }

    public void ToTitle()
    {
        Time.timeScale = 1;
        StageManager.GoTitle();
    }

}
