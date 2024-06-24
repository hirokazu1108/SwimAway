using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum MoveState
    {
        Stop,
        MoveHorizontal,
        MoveVertical,
        Invincible, // 無敵モード
    }

    // 状態変数
    [SerializeField, Tooltip("現在の状態")] private MoveState _moveState = MoveState.MoveHorizontal;
    [SerializeField, Tooltip("潜在的な方向")] private Vector2 _potentialDirection = new Vector2(1, 1);

    // 移動に利用
    private float _targetSpeed = 0.0f;
    [SerializeField, Tooltip("最大速度")] private float _maxSpeed;
    private float _speedRate = 1.0f;
    private bool _isTurning = false;
    private float _cosIdentityDirAngle = 0.0f;

    // 無敵
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

    // getter
    public float SpeedRate => _speedRate;

    // setter
    public void setSpeedRate(float rate){
        _speedRate = Mathf.Abs(rate);
    }

    // 状態を返す処理
    public bool IsInvincible => (_moveState == MoveState.Invincible);


    #region --- Unityライフサイクル ---

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _collider = GetComponent<BoxCollider>();
            SetBoundSetting();
        }

        private void Update()
        {
            if (this.IsInvincible) adjustHeight();

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
            if (_isTurning) return;

            var enemyBound = collision.gameObject.GetComponent<EnemyBound>();
            var power = enemyBound ? enemyBound.BoundPower : 1.0f;
            Bound(collision.contacts[0].point, power);
        }
    #endregion

    /// <summary>
    /// バウンド処理の設定
    /// </summary>
    private void SetBoundSetting()
    {
        const float adjustingAngle = 3f * Mathf.Deg2Rad;    // 調整用のパラメータ（一定角度の許容）
        var colliderSize = _collider.size;
        _cosIdentityDirAngle = Mathf.Cos(Mathf.Atan(colliderSize.y / colliderSize.x) + adjustingAngle);   //縦か横かを判断する角度の閾値
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
            if (_moveState == MoveState.MoveHorizontal)
            {
                _moveState = MoveState.MoveVertical;
                StartCoroutine(TurnTo(_potentialDirection));
                _inputTimer = 0;
            }
            else if (_moveState == MoveState.MoveVertical)
            {
                _moveState = MoveState.MoveHorizontal;
                StartCoroutine(TurnTo(_potentialDirection));
                _inputTimer = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {        
            EnterInvincible();
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
        // dirの向きに目標速度を維持して移動
        var power = 0f;
        if (_moveState != MoveState.Stop) power = _rb.velocity.magnitude + 1;  // 加える力目標速度に到達するまでの時間を変化させられる

        if (_maxSpeed < _rb.velocity.magnitude)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpeed;
        }
        else
        {
            var vectorAddForce = transform.right * (_targetSpeed - _rb.velocity.magnitude) * 10;
            _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// 指定した方向を向く
    /// </summary>
    /// <param name="direction">向きたい方向のベクトル</param>
    /// <returns></returns>
    private IEnumerator TurnTo(Vector2 direction)
    {
        _isTurning = true;

        //指定方向に向くための変数のセット
        Quaternion targetRot = transform.rotation;

        if (_moveState == MoveState.MoveHorizontal || _moveState == MoveState.Invincible)
        {
            if (direction.x > 0)
            {
                targetRot = Quaternion.Euler(-90, 0, 0);  //Forward
            }
            else if (direction.x < 0)
            {
                targetRot = Quaternion.Euler(-90, 0, -180); //Back
            }
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;    //x軸方向にしか移動できないようにする
        }
        else if (_moveState == MoveState.MoveVertical || _moveState == MoveState.Invincible)
        {
            if (direction.y > 0)
            {
                targetRot = Quaternion.Euler(0, -90, 90);  //Up
            }
            else if (direction.y < 0)
            {
                targetRot = Quaternion.Euler(0, 90, -90);  //Down
            }
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;     //y軸方向にしか移動できないようにする
        }

        //回転処理
        var frameCount = 0;
        const float rotateFrame = 18f; //回転にかけるフレーム数
        while (frameCount < 180f / rotateFrame)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateFrame);
            frameCount++;
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = targetRot;

        _isTurning = false;

        yield break;
    }

    /// <summary>
    /// 現在の進行方向のベクトルを取得
    /// </summary>
    /// <returns>現在の進行方向</returns>
    private Vector2 GetCurrentDirectionVector2()
    {
        if (_moveState == MoveState.MoveHorizontal)
        {
            return new Vector2(_potentialDirection.x, 0);
        }
        else if (_moveState == MoveState.MoveVertical)
        {
            return new Vector2(0, _potentialDirection.y);
        }
        else
        {
            return Vector2.zero;
        }
    }

    /// <summary>
    /// バウンド処理
    /// </summary>
    /// <param name="collidePoint">衝突した座標</param>
    /// <param name="power">バウンドの強さ</param>
    private void Bound(Vector3 collidePoint, float power = 1.0f)
    {

        Vector2 dir = (transform.position - collidePoint).normalized;        
        var cosVec = Vector2.Dot(Vector2.right, dir) / dir.magnitude;   // (1,0)とdirのなす角のcos値を求める  =（内積）/ (各ベクトルの大きさの積)


        var currentDir = GetCurrentDirectionVector2();
        var forceDir = Vector2.zero;
        if (_cosIdentityDirAngle < Mathf.Abs(cosVec))
        {
            _moveState = MoveState.MoveHorizontal;
            _potentialDirection.x = dir.x / Mathf.Abs(dir.x);
            forceDir = new Vector2(_potentialDirection.x, 0);
        }
        else
        {
            _moveState = MoveState.MoveVertical;
            _potentialDirection.y = dir.y / Mathf.Abs(dir.y);
            forceDir = new Vector2(0, _potentialDirection.y);
        }

        StartCoroutine(TurnTo(_potentialDirection));
        _rb.AddForce(forceDir * power, ForceMode.Impulse);
    }


    #region --- 無敵に関する処理 ---

        /// <summary>
        /// 無敵モードになる
        /// </summary>
        private void EnterInvincible()
        {
            if (this.IsInvincible) return;
            if (_invincibledElapsedTime < _invincibleCanUseTime) return;

            _moveState = MoveState.Invincible;
            _collider.isTrigger = true;
            _normalModel.SetActive(false);
            _invincibleModel.SetActive(true);
            setSpeedRate(_speedRate * _invincibleSpeedRate);
            StartCoroutine(TurnTo(Vector2.right));
            Invoke("ExitInvincible", _invincibleTime);  // 終了処理を登録
        }

        /// <summary>
        /// 無敵モード終了
        /// </summary>
        private void ExitInvincible()
        {
            _moveState = MoveState.MoveHorizontal;
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
        private void adjustHeight()
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
}
