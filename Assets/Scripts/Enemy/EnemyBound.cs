using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBound : MonoBehaviour
{
    [Space(10), Header("[Parameter]")]
    [SerializeField, Header("���˕Ԃ��̃N�[���^�C��")] private float _boundInterval;
    [SerializeField, Header("���˕Ԃ�̋���")] private float _boundPower;

    private bool _isBound = false;    //�o�E���h�����̃t���O
    private Rigidbody _rb = null;

    public float BoundPower { get { return _boundPower; } }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // �o�E���h��̃N�[�����Ԃ�ҋ@
    private void WaitBoundInterval()
    {
        _isBound = true;
        _rb.isKinematic = false;
        Invoke("ExitBoundState", _boundInterval);
    }

    //�o�E���h�̃N�[�����Ԃ��I��
    private void ExitBoundState()
    {
        _rb.velocity = Vector3.zero;
        _isBound = false;
        _rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        WaitBoundInterval();

        var dir = (collision.contacts[0].point - transform.position).normalized;
        _rb.AddForce(-dir * _boundPower, ForceMode.Impulse);
    }
}
