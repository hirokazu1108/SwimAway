using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFish : Enemy
{
    private enum MoveMode
    {
        XAxis,
        YAxis,
    }

    [Space(10), Header("[Parameter]")]
    [SerializeField, Header("�ړ���")] private MoveMode _moveMode;
    [SerializeField, Header("X�������̉�����")] private float _roundTripWidthX;
    [SerializeField, Header("Y�������̉�����")] private float _roundTripWidthY;
    [SerializeField, Header("���˕Ԃ�̋���")] private float _boundPower;
    [SerializeField, Header("���˕Ԃ��̃N�[���^�C��")] private float _boundInterval;
    [SerializeField, Header("�Փ˂̂��Ɛi�s�������ω����邩�ǂ���[�s����]")] private bool isSwitchingDirection;

    private Rigidbody _rb = null;
    private Vector3 _startPos = Vector3.zero;   //�����ړ��̊�ʒu
    private float _boundTimer = 99f;    //�o�E���h���Ԃ̑���
    private bool _dirTogle = false; //�i�s�����̃g�O��
    private Action moveFunc = null;


    private GameObject _targetPointObj = null;


    private void Start()
    {
        _targetPointObj = transform.GetChild(0).gameObject;

        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        moveFuncSet();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Hit(collision);
    }

    private void moveFuncSet()
    {

        switch (_moveMode)
        {
            case MoveMode.XAxis:
                moveFunc = () => {
                    var targetPos = Vector3.zero;
                    if (_dirTogle) targetPos = new Vector3(_startPos.x + _roundTripWidthX, _startPos.y, _startPos.z);
                    else targetPos = new Vector3(_startPos.x - _roundTripWidthX, _startPos.y, _startPos.z);
                    _targetPointObj.transform.position = targetPos;

                    if ((targetPos - transform.position).magnitude < .5f) _dirTogle = !_dirTogle;

                    var dir = (targetPos - _rb.position).normalized;
                    _rb.MovePosition(transform.position + dir*speed*Time.fixedDeltaTime);
                };
                break;
            case MoveMode.YAxis:
                moveFunc = () =>
                {
                    var targetPos = Vector3.zero;
                    if (_dirTogle) targetPos = new Vector3(_startPos.x, _startPos.y + _roundTripWidthY, _startPos.z);
                    else targetPos = new Vector3(_startPos.x, _startPos.y - _roundTripWidthY, _startPos.z);
                    _targetPointObj.transform.position = targetPos;

                    if ((targetPos - transform.position).magnitude < .5f) _dirTogle = !_dirTogle;

                    var dir = (targetPos - transform.position).normalized;
                    _rb.MovePosition(transform.position + dir * speed * Time.fixedDeltaTime);
                };
                break;
        }
    }

    private void switchMoveMode()
    {
        _moveMode = _moveMode == MoveMode.XAxis ? MoveMode.YAxis : MoveMode.XAxis;
        moveFuncSet();
    }

    private void switchTargetPos()
    {

    }

    public override void Move()
    {
        if (_boundTimer > _boundInterval) moveFunc();
        else _boundTimer += Time.fixedDeltaTime;
    }

    public override void Hit()
    {

    }

    private void Hit(Collision collision)
    {
        _rb.velocity = Vector3.zero;

        // �Փˈʒu���擾����
        var hitPos = collision.contacts[0].point;

        // ��������Փˈʒu�֌������x�N�g�������߂�
        Vector3 boundVec = collision.gameObject.transform.position - hitPos;

        var player = collision.gameObject.GetComponent<Player>();

        var angle = Mathf.Atan2(boundVec.y, boundVec.x) * Mathf.Rad2Deg;

        var forceDirection = Vector2.zero;

        //�G�̈ʒu�ŏ�������
        if (angle < 135 && angle < -135)    //���
        {
            forceDirection = new Vector2(-1, 0);
        }
        else if (angle < -45)   //����
        {
            forceDirection = new Vector2(0, -1);
        }
        else if (angle < 45)    //�O��
        {
            forceDirection = new Vector2(1, 0);
        }
        else //���
        {
            forceDirection = new Vector2(0, 1);
        }

        _boundTimer = 0;
        _dirTogle = !_dirTogle; //�ڕW�n�_�̕ύX
        if (collision.gameObject.CompareTag("Player")) player.addForce(forceDirection, _boundPower);
        _rb.AddForce(-forceDirection * _boundPower, ForceMode.Impulse);

        if (isSwitchingDirection) switchMoveMode(); //�����؂�ւ�
    }
}
