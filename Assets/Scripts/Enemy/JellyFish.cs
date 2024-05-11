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
        Pinned, //�ʒu�Œ�
    }

    [Space(10), Header("[Parameter]")]
    [SerializeField, Header("�ړ���")] private MoveMode _moveMode;
    [SerializeField, Header("�����ړ��̊�ʒu")] private Vector3 _basePoint = Vector3.zero;
    [SerializeField, Header("X�������̉�����")] private float _roundTripWidthX;
    [SerializeField, Header("Y�������̉�����")] private float _roundTripWidthY;
    [SerializeField, Header("���˕Ԃ�̋���")] private float _boundPower;
    [SerializeField, Header("���˕Ԃ��̃N�[���^�C��")] private float _boundInterval;
    [SerializeField, Header("�Փ˂̂��Ɛi�s�������ω����邩�ǂ���[�s����]")] private bool isSwitchingDirection;


    private Rigidbody _rb = null;

    private bool _isBound = false;    //�o�E���h�����̃t���O
    private bool _dirTogle = false; //�i�s�����̃g�O��
    private Action moveFunc = null;
    private Transform _boneTransform = null;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        if (MoveMode.Pinned == _moveMode) _basePoint = transform.position;
        _boneTransform = transform.GetChild(0).GetChild(0);
        moveFuncSet();
    }

    private void Update()
    {
        Move();
        //emitDetectRay();
    }


    private void moveFuncSet()
    {
        switch (_moveMode)
        {
            case MoveMode.XAxis:
                moveFunc = () => {
                    var targetPos = Vector3.zero;
                    if (_dirTogle)
                    {
                        targetPos = new Vector3(_basePoint.x + _roundTripWidthX, _basePoint.y, _basePoint.z);
                    }
                    else
                    {
                        targetPos = new Vector3(_basePoint.x - _roundTripWidthX, _basePoint.y, _basePoint.z);
                        
                    }


                    if ((targetPos - transform.position).magnitude < 1f) _dirTogle = !_dirTogle;

                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                };
                break;
            case MoveMode.YAxis:
                moveFunc = () =>
                {
                    var targetPos = Vector3.zero;
                    if (_dirTogle)
                    {
                        targetPos = new Vector3(_basePoint.x, _basePoint.y + _roundTripWidthY, _basePoint.z);
                    }
                    else
                    {
                        targetPos = new Vector3(_basePoint.x, _basePoint.y - _roundTripWidthY, _basePoint.z);
                    }

                    if ((targetPos - transform.position).magnitude < 1f) _dirTogle = !_dirTogle;

                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                };
                break;
            case MoveMode.Pinned:
                moveFunc = () =>
                {

                    if((_basePoint-transform.position).magnitude > .1f)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _basePoint, speed * Time.deltaTime);
                    }
                    
                };
                break;
        }
    }

    private void switchMoveMode()
    {
        if (_moveMode == MoveMode.XAxis) _moveMode = MoveMode.YAxis;
        else if (_moveMode == MoveMode.YAxis) _moveMode = MoveMode.XAxis;
        moveFuncSet();
    }

    public override void Move()
    {
        if (_isBound) return;
        moveFunc();
    }

    public override void Hit()
    {

    }

    // �o�E���h��̃N�[�����Ԃ�ҋ@
    private void waitBoundInterval()
    {
        _isBound = true;
        _rb.isKinematic = false;
        Invoke("exitBoundState", _boundInterval);
    }

    //�o�E���h�̃N�[�����Ԃ��I��
    private void exitBoundState()
    {
        _rb.velocity = Vector3.zero;
        _isBound = false;
        _rb.isKinematic = true;
    }

    //�i�s�����Ƀv���C���[�������牟��
    private void emitDetectRay()
    {
        if (_isBound) return;
        if (!_boneTransform) return;
        Debug.Log(_boneTransform.name);

        if (_moveMode == MoveMode.Pinned) return;

        var startP = _boneTransform.position + new Vector3(0, -2, 0);
        var _rayDistance = 1.5f;
        Ray ray = new Ray(startP, transform.forward);
        Debug.DrawRay(startP, transform.forward * _rayDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                waitBoundInterval();    //�N�[���^�C������

                var player = hit.collider.gameObject.GetComponent<Player>();

                player.addForce(transform.forward, _boundPower);
                _rb.AddForce(-transform.forward * _boundPower, ForceMode.Impulse);

                
                _dirTogle = !_dirTogle; //�ڕW�n�_�̕ύX

                if (isSwitchingDirection) switchMoveMode(); //�����؂�ւ�
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        waitBoundInterval();

        var dir = (collision.contacts[0].point - transform.position).normalized;

        //�o�E���h����
        if (collision.gameObject.CompareTag("Player"))
        {
            var player = collision.gameObject.GetComponent<Player>();
            player.addForce(dir * _boundPower, _boundPower);
        }
        
        _rb.AddForce(-dir* _boundPower, ForceMode.Impulse);
    }

}
