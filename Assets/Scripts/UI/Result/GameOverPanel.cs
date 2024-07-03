using System.Collections;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField, Tooltip("表示時間")] private float _visibleSeconds = 2f;

    private PopupPanel _selfPopupPanel;
    [SerializeField, Tooltip("リザルトPopupPanelコンポーネント")] private PopupPanel _resultPopupPanel;

    private void Start()
    {
        _selfPopupPanel = GetComponent<PopupPanel>();
    }
    private void OnEnable()
    {
        if (!_selfPopupPanel) return;
        StartCoroutine(Close());
    }

    private IEnumerator Close()
    {
        while (true)
        {
            if (_selfPopupPanel.IsOpen) break;
            yield return null;
        }

        yield return new WaitForSecondsRealtime(_visibleSeconds);

        _selfPopupPanel.Close(() =>
        { 
            _resultPopupPanel.Open();
        });

        yield break;
    }
}
