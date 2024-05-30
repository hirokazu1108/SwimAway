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
    /// �Q�[���o�ߎ��Ԃ̃��Z�b�g
    /// </summary>
    private void GameTimerReset()
    {
        _gameTime = 0;
    }

    /// <summary>
    /// �Q�[���o�ߎ��Ԃ��v��
    /// </summary>
    private void GameTimerCount()
    {
        _gameTime += Time.deltaTime;
    }

    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>
    public static void GameOver()
    {
        Debug.Log("GameOver");

        Retry();
    }



    /// <summary>
    /// �Q�[�����͂��߂���ǂݍ���
    /// </summary>
    public static void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// �Q�[�����ꎞ��~
    /// </summary>
    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    /// <summary>
    /// �Q�[�����ĊJ
    /// </summary>
    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
