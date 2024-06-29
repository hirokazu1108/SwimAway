using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class InvGage : MonoBehaviour
{
    [SerializeField, Tooltip("�_�ł�����Image�R���|�[�l���g")] private Image _flashImage;
    [SerializeField, Tooltip("�_�ŊԊu")] private float _flashDuration;

    private Tweener _tweener = null;


    /// <summary>
    /// �Q�[�W�̓_�ł��J�n
    /// </summary>
    public void StartFlash()
    {
        // �Đ����Ȃ�
        if (_tweener != null) return;   

        _tweener = _flashImage.DOFade(1, _flashDuration).SetLoops(-1, LoopType.Yoyo);// �s�����𖳌��ɌJ��Ԃ�
    }

    /// <summary>
    /// �Q�[�W�̓_�ł��I��
    /// </summary>
    private void EndFlash()
    {
        _tweener.Kill(true);
        _tweener = null;
    }


    /// <summary>
    /// �Q�[�W�̓_����Ԃ�ύX
    /// <param name="isOn">�_����Ԃ��ǂ���</param>
    /// </summary>
    public void SetInvincibleVisible(bool isOn)
    {
        EndFlash();

        float alpha = isOn ? 1 : 0;
        _flashImage.color = new Color(1, 1, 1, alpha);  // ��������
    }

    private void OnDestroy()
    {
        EndFlash();
    }
}
