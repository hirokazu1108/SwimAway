using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundMoveEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("巡回するポイントを集めた親オブジェクト")] private Transform _roundPoints = null;

    // 移動に利用
    [SerializeField, Tooltip("目標速度")] private float _targetSpeed = 5.0f;
    [SerializeField, Tooltip("最大速度")] private float _maxSpeed = 10;
    [SerializeField, Tooltip("目標速度に達するための力の大きさ"), Range(0, 10)] private float _reachPower = 5f;
    private const float _REACHED_DISTANNCE = 1f;
    private float _currentReachPower = 0;
    [SerializeField] private List<Vector3> _roundPointList = new List<Vector3>();
    [SerializeField] private Vector3 _targetPoint = Vector3.zero;
    private int _targetIndex = 0;

    // コンポーネント
    private Rigidbody _rb = null;


    #region --- Unityライフサイクル ---
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _currentReachPower = _reachPower;
            GenerateRoundPointList();
            //Destroy(_roundPoints.gameObject);
            DefineNextTargetPoint();
        }

        private void FixedUpdate()
        {
            if (IsReachedTargetPoint())
            {
                DefineNextTargetPoint();
            }
            else
            {
                Move();
            }
        }

    #endregion

    /// <summary>
    /// 巡回する座標リストを生成する
    /// </summary>
    private void GenerateRoundPointList()
    {

        foreach (Transform roundPoint in _roundPoints) {
            _roundPointList.Add(roundPoint.position);
        }

        if(_roundPointList.Count == 0)
        {
            _roundPointList = null;
            Debug.LogError("巡回する座標を取得できませんでした");
        }
    }

    /// <summary>
    /// 次の目標地点を決める
    /// </summary>
    private void DefineNextTargetPoint()
    {
        if (_targetIndex < _roundPointList.Count)
        {
            _targetPoint = _roundPointList[_targetIndex++];
        }
        else
        {
            _targetIndex = 0;
        }
    }

    /// <summary>
    /// 目標地点に到達したか
    /// </summary>
    /// <returns>目標地点に到達したならtrue</returns>
    private bool IsReachedTargetPoint()
    {
        var dist = (_targetPoint - transform.position).magnitude;
        return dist < _REACHED_DISTANNCE;
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        var dir = (_targetPoint - transform.position).normalized;    // 目標地点への方向ベクトル
        
        if (_maxSpeed < _rb.velocity.magnitude)
        {
            _rb.velocity = dir.normalized * _maxSpeed;
        }
        else
        {
            var vectorAddForce = dir * (_targetSpeed - _rb.velocity.magnitude) * _currentReachPower;
            _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(EvaluateReachPower());
    }

    private IEnumerator EvaluateReachPower()
    {
        var target = _reachPower;
        _currentReachPower = _reachPower / 2;

        while (true)
        {
            if (Mathf.Abs(_currentReachPower - _reachPower) < 1e-2)
            {
                break;
            }

            _currentReachPower += 0.01f;

            yield return null;
        }

        yield break;
    }
}
