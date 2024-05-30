using System.Collections;
using UnityEngine;

public class AccelerationRing : MonoBehaviour
{
    private enum AccelerateDirection
    {
        Foward,
        Reverse,
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
    }
    private GameObject _playerObj = null;

    [SerializeField, Tooltip("������")] private float _acceleRate;
    [SerializeField, Tooltip("��������")] private float _acceleTime;


    //���ݖ��g�p
    [Tooltip("��������")] private AccelerateDirection _direction;
    [Tooltip("���������邩")] private bool _canEnterSide;
    [Range(0, 90) ,Tooltip("CanEnterSide��OFF�̂Ƃ��̋��e�p�x�i�ʓx�j")] private float _allowableAngleDeg;
    private float _allowableAngleDot = 1f;
    

    private void Start()
    {
        _allowableAngleDot = Mathf.Cos(_allowableAngleDeg);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerObj = other.gameObject;
        StartCoroutine(AddAcceleEffect());

    }

    private Vector3 getAccelerateDirection()
    {
        if (_playerObj == null) return Vector3.zero;

        var player = _playerObj.GetComponent<Player>();

        switch (_direction)
        {
            case AccelerateDirection.Foward:
                return _playerObj.transform.right;
            case AccelerateDirection.Reverse:
                return -_playerObj.transform.right;
            case AccelerateDirection.PositiveX:
                return transform.right;
            case AccelerateDirection.NegativeX:
                return -transform.right;
            case AccelerateDirection.PositiveY:
                return transform.up;
            case AccelerateDirection.NegativeY:
                return -transform.up;
        }

        return Vector3.zero;
    }

    /*
     * �T�C�h����̐N���ɂ������̎��s�L��
     * �ԋp�l
     *       true : ����
     *      false : �������Ȃ�
     */
    private bool checkAccelerateByEnterSide()
    {
        if (_canEnterSide) return true; //�T�C�h����������

        var ringAdvanceDirection = transform.right;
        var playerAdvanceDirection = _playerObj.transform.right;
        var dot = Vector3.Dot(ringAdvanceDirection, playerAdvanceDirection);

        if (Mathf.Abs(dot) >= _allowableAngleDot) return true;

        return false;
    }


    /// <summary>
    /// �������ʂ�t�^
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator AddAcceleEffect()
    {
        if(_playerObj == null) yield break;

        var player = _playerObj.GetComponent<Player>();

        player.setSpeedRate(player.SpeedRate * _acceleRate);
        yield return new WaitForSeconds(_acceleTime);
        player.setSpeedRate(player.SpeedRate / _acceleRate); // ���̑��x����
        _playerObj = null;

        yield break;
    }
    
}
