using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InvGage : MonoBehaviour
{
    [SerializeField, Tooltip("点滅させるImageコンポーネント")] private Image _flashImage;
    [SerializeField, Tooltip("点滅間隔")] private float _flashDuration;

    private Tweener _tweener = null;


    /// <summary>
    /// ゲージの点滅を開始
    /// </summary>
    public void StartFlash()
    {
        // 再生中なら
        if (_tweener != null) return;   

        _tweener = _flashImage.DOFade(1, _flashDuration).SetLoops(-1, LoopType.Yoyo);// 行き来を無限に繰り返す
    }

    /// <summary>
    /// ゲージの点滅を終了
    /// </summary>
    private void EndFlash()
    {
        _tweener.Kill(true);
        _tweener = null;
    }


    /// <summary>
    /// ゲージの点灯状態を変更
    /// <param name="isOn">点灯状態かどうか</param>
    /// </summary>
    public void SetInvincibleVisible(bool isOn)
    {
        EndFlash();

        float alpha = isOn ? 1 : 0;
        _flashImage.color = new Color(1, 1, 1, alpha);  // 無透明に
    }

    private void OnDestroy()
    {
        EndFlash();
    }
}
