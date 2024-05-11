using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadWall : Enemy
{
    [SerializeField] private float _elapsedTime = 0f;    //経過時間

    private Rigidbody _rb = null;
    private float _targetSpeed;  //目標の速さ（adjustSpeed()で管理）

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        adjustSpeed();
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
}
