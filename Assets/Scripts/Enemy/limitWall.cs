using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitWall : Enemy
{
    private Rigidbody _rb = null;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        Init();
    }

    private void FixedUpdate()
    {
        Move();
    }

    protected override void Init()
    {
        
    }

    public override void Move()
    {
        float power = _rb.velocity.magnitude + 1;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

        var vectorAddForce = transform.forward * (speed - _rb.velocity.magnitude) * power;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    public override void Hit()
    {
        Debug.Log("����[���[");
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))   Hit();
    }
}
