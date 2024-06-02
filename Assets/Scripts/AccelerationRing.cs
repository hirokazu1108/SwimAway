using System.Collections;
using UnityEngine;

public class AccelerationRing : MonoBehaviour
{
    private enum AccelerateDirection
    {
        Foward,
        Reverse,
        PositiveX,
        NegativeX,
        PositiveY,
        NegativeY,
    }
    private GameObject _playerObj = null;

    [SerializeField, Tooltip("加速率")] private float _acceleRate;
    [SerializeField, Tooltip("加速時間")] private float _acceleTime;
    [SerializeField, Range(0, 90), Tooltip("侵入許容角度(度数）")] private float _allowableEnterAngle;
    private float _allowableAngleDot = 1f;


    //現在未使用
    [Tooltip("加速方向")] private AccelerateDirection _direction;

    private void Start()
    {
        _allowableAngleDot = Mathf.Cos(_allowableEnterAngle);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerObj = other.gameObject;


        if (!checkAccelerateByEnterSide()) return;
        StartCoroutine(AddAcceleEffect());

    }

    private Vector3 getAccelerateDirection()
    {
        if (_playerObj == null) return Vector3.zero;

        var player = _playerObj.GetComponent<Player>();

        switch (_direction)
        {
            case AccelerateDirection.Foward:
                return _playerObj.transform.right;
            case AccelerateDirection.Reverse:
                return -_playerObj.transform.right;
            case AccelerateDirection.PositiveX:
                return transform.right;
            case AccelerateDirection.NegativeX:
                return -transform.right;
            case AccelerateDirection.PositiveY:
                return transform.up;
            case AccelerateDirection.NegativeY:
                return -transform.up;
        }

        return Vector3.zero;
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
