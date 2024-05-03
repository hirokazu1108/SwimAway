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
    [SerializeField, Header("X軸方向の往復幅")] private float _roundTripWidthX;
    [SerializeField, Header("Y軸方向の往復幅")] private float _roundTripWidthY;
    [SerializeField, Header("跳ね返りの強さ")] private float _boundPower;
    [SerializeField, Header("跳ね返り後のクールタイム")] private float _boundInterval;
    [SerializeField, Header("衝突のあと進行方向が変化するかどうか[不安定]")] private bool isSwitchingDirection;

    private Rigidbody _rb = null;
    private Vector3 _startPos = Vector3.zero;   //反復移動の基準位置
    private bool _isBound = false;    //バウンド中かのフラグ
    private bool _dirTogle = false; //進行方向のトグル
    private Action moveFunc = null;
    private float _rayDistance = .75f;


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        moveFuncSet();
    }

    private void Update()
    {
        Move();
        emitDetectRay();
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
                        targetPos = new Vector3(_startPos.x + _roundTripWidthX, _startPos.y, _startPos.z);
                        transform.rotation = Quaternion.Euler(0,90,0);
                    }
                    else
                    {
                        targetPos = new Vector3(_startPos.x - _roundTripWidthX, _startPos.y, _startPos.z);
                        transform.rotation = Quaternion.Euler(0, -90, 0);
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
                        targetPos = new Vector3(_startPos.x, _startPos.y + _roundTripWidthY, _startPos.z);
                        transform.rotation = Quaternion.Euler(-90, 0, 0);
                    }
                    else
                    {
                        targetPos = new Vector3(_startPos.x, _startPos.y - _roundTripWidthY, _startPos.z);
                        transform.rotation = Quaternion.Euler(90, 0, 0);
                    }

                    if ((targetPos - transform.position).magnitude < 1f) _dirTogle = !_dirTogle;

                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                };
                break;
        }
    }

    private void switchMoveMode()
    {
        _moveMode = _moveMode == MoveMode.XAxis ? MoveMode.YAxis : MoveMode.XAxis;
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

    // バウンド後のクール時間を待機
    private void waitBoundInterval()
    {
        _isBound = true;
        Invoke("exitBoundState", _boundInterval);
    }

    //バウンドのクール時間を終了
    private void exitBoundState()
    {
        _rb.velocity = Vector3.zero;
        _isBound = false;
    }

    //進行方向にプレイヤーがいたら押す
    private void emitDetectRay()
    {
        if (_isBound) return;

        Ray ray = new Ray(transform.position, transform.forward);
        //Debug.DrawRay(transform.position, transform.forward * _rayDistance, Color.red);

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, _rayDistance))
        {
            if (hit.collider.CompareTag("Player"))
            {
                var player = hit.collider.gameObject.GetComponent<Player>();

                player.addForce(transform.forward, _boundPower);
                _rb.AddForce(-transform.forward * _boundPower, ForceMode.Impulse);

                waitBoundInterval();    //クールタイム処理
                _dirTogle = !_dirTogle; //目標地点の変更

                if (isSwitchingDirection) switchMoveMode(); //方向切り替え
            }
        }
    }

}
