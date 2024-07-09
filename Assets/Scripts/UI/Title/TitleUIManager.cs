using UnityEngine;
using UnityEngine.UI;

public class TitleUIManager : MonoBehaviour
{
    [SerializeField] private PopupPanel _creditPanel;
    [SerializeField] private Scrollbar _creditScrollbar;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartGame();
        }
    }

    /// <summary>
    /// ƒQ[ƒ€ŠJnˆ—
    /// </summary>
    public void StartGame()
    {
        if (StageManager.ExistsNextStage())
        {
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
