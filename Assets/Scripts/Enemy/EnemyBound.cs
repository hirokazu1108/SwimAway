using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBound : MonoBehaviour
{
    // 跳ね返り処理
    [SerializeField, Tooltip("跳ね返り後のクールタイム")] private float _boundInterval;
    [SerializeField, Tooltip("跳ね返りの強さ")] private float _boundPower;
    private bool _isBound = false;    //バウンド中かのフラグ

    // コンポーネント
    private Rigidbody _rb = null;

    // getter
    public float BoundPower => _boundPower;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        WaitBoundInterval();

        var dir = (collision.contacts[0].point - transform.position).normalized;
        _rb.AddForce(-dir * _boundPower, ForceMode.Impulse);
    }

    /// <summary>
    /// バウンド後のクール時間を待機
    /// </summary>
    private void WaitBoundInterval()
    {
        _isBound = true;
        _rb.isKinematic = false;
        Invoke("ExitBoundState", _boundInterval);
    }

    /// <summary>
    /// バウンドのクール時間を終了
    /// </summary>
    private void ExitBoundState()
    {
        _rb.velocity = Vector3.zero;
        _isBound = false;
        _rb.isKinematic = true;
    }
}
