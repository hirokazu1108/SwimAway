using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadWall : Enemy
{
    [SerializeField] private float _elapsedTime = 0f;    //経過時間
    [SerializeField] private Transform _playerTransform;

    // 目標値に到達するまでのおおよその時間[s]
    [SerializeField] private float _smoothTime = 0.3f;

    private float _maxSpeed = float.PositiveInfinity;
    private float _currentVelocity = 0;

    private Rigidbody _rb = null;
    private float _targetSpeed;  //目標の速さ（adjustSpeed()で管理）

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        adjustSpeed();
        adjustHeight();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        float power = 10;  // 加える力目標速度に到達するまでの時間を変化させられる

        var vectorAddForce = transform.forward * (_targetSpeed - _rb.velocity.magnitude) * power;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    public override void Hit()
    {
        GameManager.GameOver();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Hit();
    }

    //時間経過による速度変化
    private void adjustSpeed()
    {
        _elapsedTime += Time.deltaTime;
        _targetSpeed = Mathf.Sqrt(_elapsedTime);
    }

    private void adjustHeight()
    {
        // 現在位置取得
        var currentPos = transform.position;

        // 次フレームの位置を計算
        currentPos.y = Mathf.SmoothDamp(
            currentPos.y,
            _playerTransform.position.y,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );

        // 現在位置のx座標を更新
        transform.position = currentPos;
    }
}
