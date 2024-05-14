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
    [SerializeField, Header("移動軸")] private MoveMode _moveMode;
    [SerializeField, Header("初期位置の反復移動の基準位置とのずれ")] private Vector3 _diffBasePoint = Vector3.zero;
    [SerializeField, Header("X軸方向の往復幅")] private float _roundTripWidthX;
    [SerializeField, Header("Y軸方向の往復幅")] private float _roundTripWidthY;
    [SerializeField, Header("衝突のあと進行方向が変化するかどうか[不安定]")] private bool isSwitchingDirection;

    private bool _dirTogle = false; //進行方向のトグル
    private Action moveFunc = null;
    private Vector3 _basePoint = Vector3.zero;


    private void Start()
    {
        _basePoint = transform.position - _diffBasePoint;
        moveFuncSet();
    }

    private void Update()
    {
        Move();
    }


    private void moveFuncSet()
    {
        switch (_moveMode)
        {
            case MoveMode.XAxis:
                moveFunc = () =>
                {
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
        moveFunc();
    }

    public override void Hit()
    {

    }
}