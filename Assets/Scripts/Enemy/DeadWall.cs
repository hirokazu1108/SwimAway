using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadWall : Enemy
{
    [SerializeField] private float _elapsedTime = 0f;    //�o�ߎ���

    private Rigidbody _rb = null;
    private float _targetSpeed;  //�ڕW�̑����iadjustSpeed()�ŊǗ��j

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
        float power = 10;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

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

    //���Ԍo�߂ɂ�鑬�x�ω�
    private void adjustSpeed()
    {
        _elapsedTime += Time.deltaTime;
        _targetSpeed = Mathf.Sqrt(_elapsedTime);
    }
}
