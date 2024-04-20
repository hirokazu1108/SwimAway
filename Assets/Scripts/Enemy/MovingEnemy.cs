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


    [SerializeField] private float _roundTripWidth;   //往復の幅
    [SerializeField] private MoveMode _moveMode;    //動き方
    [SerializeField] private float boundPower;  //バウンドの強さ
    [SerializeField, Header("バウンド後のクールタイム")] private float _boundInterval;

    private Rigidbody _rb = null;
    private Vector3 _startPos;
    private float _boundTimer = 99f;

    private Action moveFunc = null;
    

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _startPos = transform.position;
        moveFuncSet();

        Init();
    }

    private void FixedUpdate()
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
                    var targetPos = new Vector3((Mathf.Sin((Time.time) * speed) * _roundTripWidth + _startPos.x), _startPos.y, _startPos.z);
                    _rb.MovePosition(targetPos);
                };
                break;
            case MoveMode.YAxis:
                moveFunc = () =>
                {
                    var targetPos = new Vector3(_startPos.x, (Mathf.Sin((Time.time) * speed) * _roundTripWidth + _startPos.y), _startPos.z);
                    _rb.MovePosition(targetPos);
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
        player.addForce(forceDirection, boundPower);
        _rb.AddForce(-forceDirection*boundPower, ForceMode.Impulse);
    }
}
