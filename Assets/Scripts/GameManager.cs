using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static float _gameTime = 0;
    private static bool _isPauseGame = false;

    // getter
    public static float GameTime => _gameTime;
    public static bool IsPauseGame => _isPauseGame;

    private void Start()
    {
        GameReset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            _isPauseGame = !_isPauseGame;
            if (_isPauseGame) PauseGame(); else ResumeGame();
        }

        if (Input.GetKeyDown(KeyCode.R))
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
        Debug.Log("GameOver");

        Retry();
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
        Time.timeScale = 0;
    }

    /// <summary>
    /// ゲームを再開
    /// </summary>
    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
