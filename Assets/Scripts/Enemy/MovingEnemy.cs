using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : Enemy
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
    private float _boundTimer = 99f;    //バウンド時間の測定
    private bool _dirTogle = false; //進行方向のトグル
    private Vector3 _velocity = Vector3.zero;
    private Action moveFunc = null;
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        moveFuncSet();

        Init();
    }

    private void Update()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        hitPlayer(collision);
    }


    protected override void Init()
    {
        
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

                    Debug.Log((targetPos - transform.position).magnitude);
                    if((targetPos-transform.position).magnitude < 1f)
                    {
                        _dirTogle = !_dirTogle;
                        Debug.Log($"{_dirTogle}:{transform.position.x}");
                    }
                    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
                };
                break;
            case MoveMode.YAxis:
                moveFunc = () =>
                {
                    var targetPos = Vector3.zero;
                    if (_dirTogle) targetPos = new Vector3(_startPos.x, _startPos.y + _roundTripWidthY, _startPos.z);
                    else targetPos = new Vector3(_startPos.x, _startPos.y - _roundTripWidthY, _startPos.z);

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
        if (_boundTimer > _boundInterval) moveFunc();
        else _boundTimer += Time.deltaTime;
    }

    public override void Hit()
    {
        
    }

    private void hitPlayer(Collision collision)
    {
        _rb.velocity = Vector3.zero;

        // 衝突位置を取得する
        var hitPos = collision.contacts[0].point;

        // 自分から衝突位置へ向かうベクトルを求める
        Vector3 boundVec = collision.gameObject.transform.position - hitPos;

        var player = collision.gameObject.GetComponent<Player>();

        var angle = Mathf.Atan2(boundVec.y, boundVec.x) * Mathf.Rad2Deg;

        var forceDirection = Vector2.zero;

        //敵の位置で条件分け
        if (angle < 135 && angle < -135)    //後方
        {
            forceDirection = new Vector2(-1, 0);
        }
        else if (angle < -45)   //下方
        {
            forceDirection = new Vector2(0, -1);   
        }
        else if (angle < 45)    //前方
        {
            forceDirection = new Vector2(1, 0);
        }
        else //上方
        {
            forceDirection = new Vector2(0, 1);
        }

        _boundTimer = 0;
        player.addForce(forceDirection, _boundPower);
        _rb.AddForce(-forceDirection*_boundPower, ForceMode.Impulse);

        if (isSwitchingDirection) switchMoveMode(); //方向切り替え
    }
}
