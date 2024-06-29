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

    [SerializeField, Tooltip("�J�n����")] private float _initTime = 0f;
    [SerializeField, Tooltip("�v���C���[�̃V�[�����L�f�[�^")] private PlayerSharedData _sharedData;

    // getter
    public static float GameTime => _gameTime;
    public static bool IsPauseGame => _isPauseGame;

    #region --- Unity���C�t�T�C�N�� ---

        private void Awake()
        {
            var canvas = transform.GetChild(0);
            if (canvas.Find("GameClearPanel") == null) Debug.LogError("gameClearPanel���A�^�b�` or ���т܂ŘA��");

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
    /// �Q�[����Ԃ̃��Z�b�g
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
        pausePanel.SetActive(false);
        gameOverPanel.Open(() =>
        {
            Time.timeScale = 0;
            _isPauseGame = true;
        });
    }
    
    /// <summary>
    /// �Q�[���N���A����
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
        _isPauseGame = true;
        pausePanel.Open(() => {
                
            Time.timeScale = 0; 
        });
    }

    /// <summary>
    /// �Q�[�����ĊJ
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
