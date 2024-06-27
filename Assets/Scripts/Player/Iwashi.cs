using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class Iwashi : MonoBehaviour
{
    public enum MoveState
    {
        Stop,
        Horizontal,
        Vertical,
    }

    // 状態変数
    [SerializeField, Tooltip("現在の状態")] private MoveState _moveState = MoveState.Horizontal;
    [SerializeField, Tooltip("潜在的な方向")] private Vector3 _potentialDirection = new Vector3(1, 1, 0);

    // 移動に利用
    private float _targetSpeed = 0.0f;
    [SerializeField, Tooltip("最大速度")] private float _maxSpeed = 10;
    private float _speedRate = 1.0f;
    [SerializeField, Tooltip("目標速度に達するための力の大きさ"),Range(0, 10)] private float _reachPower = 5f;
    private float _currentReachPower = 0;


    // 無敵
    private bool _isInvincible = false;
    private float _invincibledElapsedTime = 0.0f;   // 無敵を使ってからの時間
    [SerializeField, Tooltip("無敵を使えるまでの時間")] private float _invincibleCanUseTime;
    [SerializeField, Tooltip("無敵時間")] private float _invincibleTime;
    [SerializeField, Tooltip("無敵中の速度倍率")] private float _invincibleSpeedRate;
    [SerializeField, Tooltip("目標値に到達するまでのおおよその時間[s]")] private float _invincibleSmoothTime;
    private float _currentInvincibleVelocity = 0;

    // 入力
    [SerializeField, Tooltip("キー入力の受付間隔")] private float _inputInterval;
    private float _inputTimer = 99f;

    // コンポーネント
    private Rigidbody _rb = null;
    private BoxCollider _collider = null;
    [SerializeField, Tooltip("通常モデルのオブジェクト")] private GameObject _normalModel;
    [SerializeField, Tooltip("無敵モデルのオブジェクト")] private GameObject _invincibleModel;
    [SerializeField, Tooltip("プレイヤーのシーン共有データ")] private PlayerSharedData _sharedData;

    private List<Vector3> _debugDrawList = new List<Vector3>();

    // getter
    public float SpeedRate => _speedRate;
    public bool IsInvincible => _isInvincible;

    // setter
    public void setSpeedRate(float rate)
    {
        _speedRate = Mathf.Abs(rate);
    }

    private Vector3 GetCurrentDir()
    {
        if (_moveState == MoveState.Horizontal)
        {
            return new Vector3(_potentialDirection.x, 0, 0);
        }
        else if (_moveState == MoveState.Vertical)
        {
            return new Vector3(0, _potentialDirection.y, 0);
        }

        return Vector3.zero;
    }

    #region --- Unityライフサイクル ---

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        _currentReachPower = _reachPower;
    }

    private void Update()
    {
        if (this.IsInvincible) AdjustHeight();

        AdjustSpeed();
        UserInput();
        InvincibleTimer();
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 最も近い点を取得
        var closePoint = collision.collider.ClosestPoint(transform.position);
        var boundVec = transform.position - closePoint;

        StartCoroutine(EvaluateReachPower());        
        AddForceAndChangeDirection(boundVec.normalized);
    }
    #endregion

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var d in _debugDrawList)
        {
            
            Gizmos.DrawSphere(d, 0.1f);
        }
        
    }

    /// <summary>
    /// ユーザの入力受付メソッド
    /// </summary>
    private void UserInput()
    {
        if (GameManager.IsPauseGame) return;

        //キー入力間隔を適応
        _inputTimer += Time.deltaTime;
        if (_inputTimer < _inputInterval) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchMoveState();
            AddForceAndChangeDirection(GetCurrentDir());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            EnterInvincible();
        }
    }

    private void SwitchMoveState()
    {
        if (_moveState == MoveState.Horizontal)
        {
            _moveState = MoveState.Vertical;
        }
        else if (_moveState == MoveState.Vertical)
        {
            _moveState = MoveState.Horizontal;
        }
    }

    /// <summary>
    /// 時間経過による速度変化
    /// </summary>
    private void AdjustSpeed()
    {
        _targetSpeed = Mathf.Sqrt(GameManager.GameTime) + 1;    // 時間による速度変化

        _targetSpeed *= _speedRate; // 速度調整を行う

        _targetSpeed = Mathf.Min(_targetSpeed, _maxSpeed);  // 最大速度を超えないように
    }

    private void Move()
    {
        if (_maxSpeed < _rb.velocity.magnitude)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpeed;
        }
        else
        {
            var vectorAddForce = GetCurrentDir() * (_targetSpeed - _rb.velocity.magnitude) * _currentReachPower;
            _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// 力を加えて進行方向を変更
    /// </summary>
    /// <param name="force"></param>
    private void AddForceAndChangeDirection(Vector3 force)
    {
        _rb.velocity = Vector3.zero;
        ChangeAdvanceDirection(force.normalized);
        _rb.AddForce(GetCurrentDir(), ForceMode.Impulse);
    }


    /// <summary>
    /// 進行方向の変更
    /// </summary>
    /// <param name="dir">向く方向</param>
    private void ChangeAdvanceDirection(Vector3 dir)
    {
        dir = dir.normalized;
        var cos = Vector3.Dot(dir, Vector3.right);
        var cosBase = Mathf.Cos(16 * Mathf.Deg2Rad);    // 基準値　いわしのコライダーの縦横比から角度を抽出  theta = arctan(collider.scale.y / collider.scale.y) = 16

        if (Mathf.Abs(cos) < Mathf.Abs(cosBase))
        {
            // 縦方向
            _potentialDirection.y = dir.y < 0 ? -1 : 1;
            _moveState = MoveState.Vertical;
        }
        else
        {
            // 横方向
            _potentialDirection.x = dir.x < 0 ? -1 : 1;
            _moveState = MoveState.Horizontal;
        }

        /*
        if (Mathf.Abs(dir.x) < Mathf.Abs(dir.y))
        {
            // 縦方向
            _potentialDirection.y = dir.y < 0 ? -1 : 1;
            _moveState = MoveState.Vertical;
        }
        else
        {
            // 横方向
            _potentialDirection.x = dir.x < 0 ? -1 : 1;
            _moveState = MoveState.Horizontal;
        }*/

        transform.rotation = Quaternion.Euler(ConvertDirectionToEuler(GetCurrentDir()));
    }

    /// <summary>
    /// 進行方向の単位ベクトルからオイラー角に変換
    /// </summary>
    /// <param name="dir">進行方向の単位ベクトル</param>
    /// <returns></returns>
    private Vector3 ConvertDirectionToEuler(Vector3 dir)
    {
        dir = dir.normalized;

        if (dir == Vector3.right)
        {
            return new Vector3(0f, 0f, 0f);
        }
        else if (dir == -Vector3.right)
        {
            return new Vector3(0f, 180f, 0f);
        }
        else if (dir == Vector3.up)
        {
            return new Vector3(0f, 0f, 90f);
        }
        else if (dir == -Vector3.up)
        {
            return new Vector3(0f, 0f, -90f);
        }
        return Vector3.zero;
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

    #region --- 無敵に関する処理 ---

    /// <summary>
    /// 無敵モードになる
    /// </summary>
    private void EnterInvincible()
    {
        if (this.IsInvincible) return;
        if (_invincibledElapsedTime < _invincibleCanUseTime) return;

        _isInvincible = true;
        _collider.isTrigger = true;
        _normalModel.SetActive(false);
        _invincibleModel.SetActive(true);
        setSpeedRate(_speedRate * _invincibleSpeedRate);
        AddForceAndChangeDirection(Vector3.right);
        Invoke("ExitInvincible", _invincibleTime);  // 終了処理を登録
    }

    /// <summary>
    /// 無敵モード終了
    /// </summary>
    private void ExitInvincible()
    {
        _isInvincible = false;
        _collider.isTrigger = false;
        _normalModel.SetActive(true);
        _invincibleModel.SetActive(false);
        setSpeedRate(_speedRate / _invincibleSpeedRate);
        _invincibledElapsedTime = 0;
    }

    private void InvincibleTimer()
    {
        _invincibledElapsedTime += Time.deltaTime;
    }

    /// <summary>
    /// 無敵時の高さの調整
    /// </summary>
    private void AdjustHeight()
    {
        // 現在位置取得
        var currentPos = transform.position;
        var targetHeight = 0f;

        // 次フレームの位置を計算
        currentPos.y = Mathf.SmoothDamp(
            currentPos.y,
            targetHeight,
            ref _currentInvincibleVelocity,
            _invincibleSmoothTime,
            _maxSpeed
        );

        // 現在位置のx座標を更新
        transform.position = currentPos;
    }
    #endregion


    #region --- コインの取得処理 ---
    public void GetCoin()
    {
        _sharedData.GetCoin();
    }

    #endregion
}
