using System.Collections;
using UnityEngine;

public class AccelerationRing : MonoBehaviour
{
    private GameObject _iwashiObject = null;

    [SerializeField, Tooltip("������")] private float _acceleRate;
    [SerializeField, Tooltip("��������")] private float _acceleTime;
    [SerializeField, Range(0, 90), Tooltip("�N�����e�p�x(�x���j")] private float _allowableEnterAngle;
    private float _allowableAngleDot = 1f;

    private void Start()
    {
        _allowableAngleDot = Mathf.Cos(_allowableEnterAngle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.gameObject.GetComponent<Iwashi>().IsInvincible) return;

        _iwashiObject = other.gameObject;

        if (!checkAccelerateByEnterSide()) return;
        StartCoroutine(AddAcceleEffect());

    }

    /// <summary>
    /// �T�C�h����̐N���ɂ������̎��s�L��
    /// </summary>
    /// <returns> �������邩�ǂ���</returns>
    private bool checkAccelerateByEnterSide()
    {
        if (_iwashiObject == null) return false;

        var ringAdvanceDirection = transform.right;
        var iwashiAdvanceDirection = _iwashiObject.transform.right;
        var dot = Vector3.Dot(ringAdvanceDirection, iwashiAdvanceDirection);

        if (_allowableAngleDot <= Mathf.Abs(dot)) return true;

        return false;
    }

    /// <summary>
    /// �������ʂ�t�^
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator AddAcceleEffect()
    {
        if(_iwashiObject == null) yield break;

        var iwashi = _iwashiObject.GetComponent<Iwashi>();

        iwashi.setSpeedRate(iwashi.SpeedRate * _acceleRate);
        yield return new WaitForSeconds(_acceleTime);
        iwashi.setSpeedRate(iwashi.SpeedRate / _acceleRate); // ���̑��x����
        _iwashiObject = null;

        yield break;
    }
    
}
