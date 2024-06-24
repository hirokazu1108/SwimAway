using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TextFade : MonoBehaviour
{
    [SerializeField] Ease _easeType = Ease.Unset;
    [SerializeField, Tooltip("�t�F�C�h�Ԋu")] private float _fadeDurationSeconds = 1;
    [SerializeField, Tooltip("�����x�̍ŏ��l"), Range(0, 1)] private float _minAlpha = 0;
    [SerializeField, Tooltip("�����x�̍ő�l"), Range(0, 1)] private float _maxAlpha = 1;

    private Tweener tweener = null;

    private void Start()
    {
        var text = GetComponent<Text>();
       tweener = text.DOFade(_minAlpha, _maxAlpha).SetEase(_easeType).SetLoops(-1, LoopType.Yoyo);// �s�����𖳌��ɌJ��Ԃ�
    }

    private void OnDestroy()
    {
        StopFade();
    }

    /// <summary>
    /// �e�L�X�g�̃t�F�C�h���~�߂�
    /// </summary>
    private void StopFade()
    {
        tweener.Kill(true);
    }
}
