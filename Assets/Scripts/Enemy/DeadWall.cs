using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadWall : Enemy
{
    private Rigidbody _rb = null;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        float power = _rb.velocity.magnitude + 1;  // ‰Á‚¦‚é—Í–Ú•W‘¬“x‚É“ž’B‚·‚é‚Ü‚Å‚ÌŽžŠÔ‚ð•Ï‰»‚³‚¹‚ç‚ê‚é

        var vectorAddForce = transform.forward * (speed - _rb.velocity.magnitude) * power;
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
}
