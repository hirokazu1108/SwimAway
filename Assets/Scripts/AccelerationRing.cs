using System.Collections;
using System.Collections.Generic;
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

    [Space(10), Header("[Parameter]")]

    [SerializeField, Header("加速力")] private float _power;
    [SerializeField, Header("加速方向")] private AccelerateDirection _direction;
    [SerializeField, Header("横から入れるか")] private bool _canEnterSide;
    [SerializeField, Range(0, 90) ,Header("CanEnterSideがOFFのときの許容角度（弧度）")] private float _allowableAngleDeg;


    private GameObject _playerObj = null;
    private float _allowableAngleDot = 1f;

    private void Start()
    {
        _allowableAngleDot = Mathf.Cos(_allowableAngleDeg);
    }

    private IEnumerator acceleratePlayer()
    {
        if (_playerObj == null) yield break;
        if (!checkAccelerateByEnterSide()) yield break;

        var dir = getAccelerateDirection();
        var player = _playerObj.GetComponent<Player>();
        var time = 0f;

        //継続的に力を加える
        while (time < 5)
        {
            player.addForce(dir, _power);
            time += 1;
            yield return null;
        }

        _playerObj = null;

        yield break;
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

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        _playerObj = other.gameObject;
        StartCoroutine(acceleratePlayer());
    }
    
}
