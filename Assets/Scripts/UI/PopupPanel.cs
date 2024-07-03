using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PopupPanel : MonoBehaviour
{
    [SerializeField, Tooltip("ポップアップにかかる時間")] private float _duration = 1f;
    [SerializeField, Tooltip("ポップアップの最大スケール")] private Vector3 _maxSize = Vector3.one;

    private bool _isOpen = false;
    public bool IsOpen => _isOpen;

    private Tweener _openTweener = null;

    private void Start()
    {
        transform.localScale = Vector3.zero;
    }

    public void Open(UnityAction callback = default)
    {
        if (_isOpen) return;
        SetActive(true);
        _openTweener = transform.DOScale(_maxSize, _duration)
                                .SetUpdate(true)
                                .OnComplete(() => {
                                    _isOpen = true;
                                    callback();
                                });
    }

    public void Close(UnityAction callback = default)
    {
        if (!_isOpen) return;

        _openTweener.Complete();
        transform.DOScale(Vector3.zero, _duration)
                 .SetUpdate(true)
                 .OnComplete(() => {
                     _isOpen = false;
                     SetActive(false);

                     callback();
                 });
    }

    public void SetActive(bool isOn)
    {
        gameObject.SetActive(isOn);
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
