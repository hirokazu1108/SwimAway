using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBound : MonoBehaviour
{
    [Space(10), Header("[Parameter]")]
    [SerializeField, Header("跳ね返り後のクールタイム")] private float _boundInterval;
    [SerializeField, Header("跳ね返りの強さ")] private float _boundPower;

    private bool _isBound = false;    //バウンド中かのフラグ
    private Rigidbody _rb = null;

    public float BoundPower { get { return _boundPower; } }

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // バウンド後のクール時間を待機
    private void WaitBoundInterval()
    {
        _isBound = true;
        _rb.isKinematic = false;
        Invoke("ExitBoundState", _boundInterval);
    }

    //バウンドのクール時間を終了
    private void ExitBoundState()
    {
        _rb.velocity = Vector3.zero;
        _isBound = false;
        _rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        WaitBoundInterval();

        var dir = (collision.contacts[0].point - transform.position).normalized;
        _rb.AddForce(-dir * _boundPower, ForceMode.Impulse);
    }
}
