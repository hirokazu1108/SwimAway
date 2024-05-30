using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static float _gameTime = 0;
    private bool isPauseGame = false;

    // getter
    public static float GameTime => _gameTime;

    private void Start()
    {
        GameTimerReset();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPauseGame = !isPauseGame;
            if (isPauseGame) PauseGame(); else ResumeGame();
        }

        GameTimerCount();
    }

    /// <summary>
    /// ゲーム経過時間のリセット
    /// </summary>
    private void GameTimerReset()
    {
        _gameTime = 0;
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
