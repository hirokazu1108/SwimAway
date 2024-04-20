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

    private Rigidbody _rb = null;
    private Vector3 startPos;

    private Action moveFunc = null;
    private bool _isBound = false;    //跳ね返り中かどうか

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        startPos = transform.position;
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
                    var targetPos = new Vector3((Mathf.Sin((Time.time) * speed) * _roundTripWidth + startPos.x), startPos.y, startPos.z);
                    _rb.MovePosition(targetPos);
                };
                break;
            case MoveMode.YAxis:
                moveFunc = () =>
                {
                    var targetPos = new Vector3(startPos.x, (Mathf.Sin((Time.time) * speed) * _roundTripWidth + startPos.y), startPos.z);
                    _rb.MovePosition(targetPos);
                };
                break;
        }
    }

    public override void Move()
    {
        if (!_isBound) moveFunc();
        else
        {
            startPos = transform.position;
            if(_rb.velocity.magnitude < 1f) _isBound = false;
        } 
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


        _isBound = true;
        player.addForce(forceDirection, boundPower);
        _rb.AddForce(-forceDirection*boundPower, ForceMode.Impulse);
    }
}
