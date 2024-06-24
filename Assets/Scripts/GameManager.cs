using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static Canvas gameOverCanvas;
    private static Canvas pauseCanvas;

    private static float _gameTime = 0;
    private static bool _isPauseGame = false;

    [SerializeField, Tooltip("�v���C���[�̃V�[�����L�f�[�^")] private PlayerSharedData _sharedData;

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
    /// �Q�[����Ԃ̃��Z�b�g
    /// </summary>
    private void GameReset()
    {
        _gameTime = 0;
        _sharedData.Reset();
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
        PauseGame(false);
        gameOverCanvas.enabled = true;
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
    public static void PauseGame(bool enabelCanvas = true)
    {
        Time.timeScale = 0;
        pauseCanvas.enabled = enabelCanvas;
    }

    /// <summary>
    /// �Q�[�����ĊJ
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
