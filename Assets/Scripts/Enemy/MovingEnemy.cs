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
    [SerializeField] private MoveMode _moveMode;    //動き方
    [SerializeField] private float _roundTripWidth;   //往復の幅
    [SerializeField] private float _boundPower;  //バウンドの強さ
    [SerializeField, Header("バウンド後のクールタイム")] private float _boundInterval;

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
                    if (_dirTogle) targetPos = new Vector3(_startPos.x + _roundTripWidth, _startPos.y, _startPos.z);
                    else targetPos = new Vector3(_startPos.x - _roundTripWidth, _startPos.y, _startPos.z);

                    if((targetPos-transform.position).magnitude < 1f) _dirTogle = !_dirTogle;

                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, _roundTripWidth /speed);
                };
                break;
            case MoveMode.YAxis:
                moveFunc = () =>
                {
                    var targetPos = Vector3.zero;
                    if (_dirTogle) targetPos = new Vector3(_startPos.x, _startPos.y + _roundTripWidth, _startPos.z);
                    else targetPos = new Vector3(_startPos.x, _startPos.y - _roundTripWidth, _startPos.z);

                    if ((targetPos - transform.position).magnitude < 1f) _dirTogle = !_dirTogle;

                    transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, _roundTripWidth / speed);
                };
                break;
        }
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

        if (angle < 135 && angle < -135)
        {
            forceDirection = new Vector2(-1, 0);
        }
        else if (angle < -45)
        {
            forceDirection = new Vector2(0, -1);
        }
        else if (angle < 45)
        {
            forceDirection = new Vector2(1, 0);
        }
        else
        {
            forceDirection = new Vector2(0, 1);
        }


        _boundTimer = 0;
        player.addForce(forceDirection, _boundPower);
        _rb.AddForce(-forceDirection*_boundPower, ForceMode.Impulse);
    }
}
