using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;


public class PopButton : MonoBehaviour
{
    [SerializeField, Tooltip("最大スケール")] private float _popScale = .1f;
    [SerializeField, Tooltip("最大スケール")] private float _popDuration = .2f;
    [SerializeField, Tooltip("ポップ終了後の処理")] private UnityEvent callbackEvent;

    private Tweener _tweener = null;

    [SerializeField] private AudioClip _clickAudioClip;
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GameObject.Find("Music")?.GetComponent<AudioSource>();
        if (!_audioSource)
        {
            Debug.Log("Audiosoruce is not found.");
        }
    }


    public void OnClick()
    {
        if (_tweener != null)
        {
            _tweener.Kill(false);
            _tweener = null;
            transform.localScale = Vector3.one;
        }

        _audioSource?.PlayOneShot(_clickAudioClip);

        _tweener = transform.DOPunchScale(
            punch: Vector3.one * _popScale,
            duration: _popDuration,
            vibrato: 1
        ).SetEase(Ease.OutExpo)
         .SetUpdate(true);

        _tweener.OnComplete(() => {
             callbackEvent.Invoke();
            _tweener = null;
        });

    }

    private void OnDestroy()
    {
        _tweener.Kill();
        _tweener = null;
    }
}
