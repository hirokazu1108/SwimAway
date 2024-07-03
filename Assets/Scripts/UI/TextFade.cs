using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextFade : MonoBehaviour
{
    [SerializeField] Ease _easeType = Ease.Unset;
    [SerializeField, Tooltip("フェイド間隔")] private float _fadeDurationSeconds = 1;
    [SerializeField, Tooltip("透明度の最小値"), Range(0, 1)] private float _minAlpha = 0;

    private Tweener _tweener = null;

    private void Start()
    {
        var text = GetComponent<Text>();
        _tweener = text.DOFade(_minAlpha, _fadeDurationSeconds).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);// 行き来を無限に繰り返す
    }

    private void OnDestroy()
    {
        StopFade();
    }

    /// <summary>
    /// テキストのフェイドを止める
    /// </summary>
    private void StopFade()
    {
        _tweener.Kill(true);
    }
}
