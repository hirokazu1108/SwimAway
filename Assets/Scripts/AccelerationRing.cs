using System.Collections;
using UnityEngine;

public class AccelerationRing : MonoBehaviour
{
    private GameObject _playerObj = null;

    [SerializeField, Tooltip("加速率")] private float _acceleRate;
    [SerializeField, Tooltip("加速時間")] private float _acceleTime;
    [SerializeField, Range(0, 90), Tooltip("侵入許容角度(度数）")] private float _allowableEnterAngle;
    private float _allowableAngleDot = 1f;

    private void Start()
    {
        _allowableAngleDot = Mathf.Cos(_allowableEnterAngle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (other.gameObject.GetComponent<Player>().IsInvincible) return;

        _playerObj = other.gameObject;

        if (!checkAccelerateByEnterSide()) return;
        StartCoroutine(AddAcceleEffect());

    }

    /// <summary>
    /// サイドからの侵入による加速の実行有無
    /// </summary>
    /// <returns> 加速するかどうか</returns>
    private bool checkAccelerateByEnterSide()
    {
        if (_playerObj == null) return false;

        var ringAdvanceDirection = transform.right;
        var playerAdvanceDirection = _playerObj.transform.right;
        var dot = Vector3.Dot(ringAdvanceDirection, playerAdvanceDirection);

        if (_allowableAngleDot <= Mathf.Abs(dot)) return true;

        return false;
    }

    /// <summary>
    /// 加速効果を付与
    /// </summary>
    /// <returns>IEnumerator</returns>
    private IEnumerator AddAcceleEffect()
    {
        if(_playerObj == null) yield break;

        var player = _playerObj.GetComponent<Player>();

        player.setSpeedRate(player.SpeedRate * _acceleRate);
        yield return new WaitForSeconds(_acceleTime);
        player.setSpeedRate(player.SpeedRate / _acceleRate); // 元の速度率へ
        _playerObj = null;

        yield break;
    }
    
}
