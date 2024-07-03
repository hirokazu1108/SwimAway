using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;


public class PopButton : MonoBehaviour
{
    [SerializeField, Tooltip("�ő�X�P�[��")] private float _popScale = .1f;
    [SerializeField, Tooltip("�ő�X�P�[��")] private float _popDuration = .2f;
    [SerializeField, Tooltip("�|�b�v�I����̏���")] private UnityEvent callbackEvent;

    private Tweener _tweener = null;

    public void OnClick()
    {
        if (_tweener != null)
        {
            _tweener.Kill(false);
            _tweener = null;
            transform.localScale = Vector3.one;
        }

        _tweener = transform.DOPunchScale(
            punch: Vector3.one * _popScale,
            duration: _popDuration,
            vibrato: 1
        ).SetEase(Ease.OutExpo)
         .SetUpdate(true);

        _tweener.OnComplete(() => {
             callbackEvent.Invoke();
        });

    }



    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
