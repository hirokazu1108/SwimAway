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
    /// �Q�[����Ԃ̃��Z�b�g
    /// </summary>
    private void GameReset()
    {
        _gameTime = 0;
        _isPauseGame = false;
        ResumeGame();
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
