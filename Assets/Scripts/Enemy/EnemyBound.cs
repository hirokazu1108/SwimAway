using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBound : MonoBehaviour
{
    // ���˕Ԃ菈��
    [SerializeField, Tooltip("���˕Ԃ��̃N�[���^�C��")] private float _boundInterval;
    [SerializeField, Tooltip("���˕Ԃ�̋���")] private float _boundPower;
    private bool _isBound = false;    //�o�E���h�����̃t���O

    // �R���|�[�l���g
    private Rigidbody _rb = null;

    // getter
    public float BoundPower => _boundPower;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        WaitBoundInterval();

        var dir = (collision.contacts[0].point - transform.position).normalized;
        _rb.AddForce(-dir * _boundPower, ForceMode.Impulse);
    }

    /// <summary>
    /// �o�E���h��̃N�[�����Ԃ�ҋ@
    /// </summary>
    private void WaitBoundInterval()
    {
        _isBound = true;
        _rb.isKinematic = false;
        Invoke("ExitBoundState", _boundInterval);
    }

    /// <summary>
    /// �o�E���h�̃N�[�����Ԃ��I��
    /// </summary>
    private void ExitBoundState()
    {
        _rb.velocity = Vector3.zero;
        _isBound = false;
        _rb.isKinematic = true;
    }
}
