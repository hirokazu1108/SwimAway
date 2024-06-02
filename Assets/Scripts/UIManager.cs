using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel;

    public void openGameOverPanel()
    {
        _gameOverPanel.SetActive(true);
    }

    public void OnClickRetry()
    {
        _gameOverPanel.SetActive(false);
        GameManager.Retry();
    }
}
