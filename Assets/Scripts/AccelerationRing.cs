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


    //現在未使用
    [Tooltip("加速方向")] private AccelerateDirection _direction;
    [Tooltip("横から入れるか")] private bool _canEnterSide;
    [Range(0, 90) ,Tooltip("CanEnterSideがOFFのときの許容角度（弧度）")] private float _allowableAngleDeg;
    private float _allowableAngleDot = 1f;
    

    private void Start()
    {
        _allowableAngleDot = Mathf.Cos(_allowableAngleDeg);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerObj = other.gameObject;
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

    /*
     * サイドからの侵入による加速の実行有無
     * 返却値
     *       true : 加速
     *      false : 加速しない
     */
    private bool checkAccelerateByEnterSide()
    {
        if (_canEnterSide) return true; //サイドからも入れる

        var ringAdvanceDirection = transform.right;
        var playerAdvanceDirection = _playerObj.transform.right;
        var dot = Vector3.Dot(ringAdvanceDirection, playerAdvanceDirection);

        if (Mathf.Abs(dot) >= _allowableAngleDot) return true;

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
