using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private PopupPanel _creditPanel;
    [SerializeField] private Scrollbar _creditScrollbar;

    [SerializeField] private AudioClip _clickAudioClip;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GameObject.Find("Music").GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// ÉQÅ[ÉÄäJénèàóù
    /// </summary>
    public void StartGame()
    {
        if (StageManager.ExistsNextStage())
        {
            _audioSource.PlayOneShot(_clickAudioClip);
            StageManager.GoNextStage();
        }
    }

    public void OnClickCredit()
    {
        _creditPanel.Open(() =>
        {
            _creditScrollbar.value = 1;
        });
    }

    public void OnClickCreditClose()
    {
        _creditPanel.Close();
    }
}
