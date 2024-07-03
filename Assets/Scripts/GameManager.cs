using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private PopupPanel gameOverPanel;
    [SerializeField] private PopupPanel gameResultPanel;
    [SerializeField] private PopupPanel pausePanel;

    private float _gameTime = 0;
    private bool _isPauseGame = false;

    [SerializeField, Tooltip("�J�n����")] private float _initTime = 0f;
    [SerializeField, Tooltip("�v���C���[�̃V�[�����L�f�[�^")] private PlayerSharedData _sharedData;

    // getter
    public float GameTime => _gameTime;
    public bool IsPauseGame => _isPauseGame;

    #region --- Unity���C�t�T�C�N�� ---

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
        //���X�e�[�W�̂�
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
    /// �Q�[���o�ߎ��Ԃ��v��
    /// </summary>
    private void GameTimerCount()
    {
        _gameTime += Time.deltaTime;
    }

    /// <summary>
    /// �Q�[���I�[�o�[����
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
    /// �Q�[���N���A����
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
    /// �Q�[�����͂��߂���ǂݍ���
    /// </summary>
    public void Retry()
    {
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        StageManager.GoStageAt(0);
    }

    /// <summary>
    /// �Q�[�����ꎞ��~
    /// </summary>
    public void PauseGame()
    {
        _isPauseGame = true;
        pausePanel.Open(() => {
                
            Time.timeScale = 0; 
        });
    }

    /// <summary>
    /// �Q�[�����ĊJ
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
