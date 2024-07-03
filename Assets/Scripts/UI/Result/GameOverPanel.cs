using System.Collections;
using UnityEngine;

public class GameOverPanel : MonoBehaviour
{
    [SerializeField, Tooltip("�\������")] private float _visibleSeconds = 2f;

    private PopupPanel _selfPopupPanel;
    [SerializeField, Tooltip("���U���gPopupPanel�R���|�[�l���g")] private PopupPanel _resultPopupPanel;

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
