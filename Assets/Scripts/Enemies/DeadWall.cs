using UnityEngine;

public class DeadWall : Enemy
{

    // 位置
    [SerializeField, Tooltip("高さのオフセット")] private float _heightOffset;

    // 移動に利用
    private float _targetSpeed;
    [SerializeField, Tooltip("目標値に到達するまでのおおよその時間[s]")] private float _smoothTime;
    private float _maxSpeed = float.PositiveInfinity;
    private float _currentVelocity = 0;

    // コンポーネント
    private Rigidbody _rb = null;
    private Transform _playerTransform = null;
    private GameManager _gameManager;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void Update()
    {
        adjustSpeed();
        adjustHeight();
    }

    private void FixedUpdate()
    {
        Move();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Hit();
    }


    public override void Move()
    {
        float power = 10;  // 加える力目標速度に到達するまでの時間を変化させられる

        var vectorAddForce = transform.forward * (_targetSpeed - _rb.velocity.magnitude) * power;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    public override void Hit()
    {
        _gameManager.GameOver();
    }


    /// <summary>
    /// 時間経過による速度変化
    /// </summary>
    private void adjustSpeed()
    {
        _targetSpeed = Mathf.Sqrt(_gameManager.GameTime);
    }

    /// <summary>
    /// プレイヤー位置に伴った高さの調整
    /// </summary>
    private void adjustHeight()
    {
        // 現在位置取得
        var currentPos = transform.position;

        // 次フレームの位置を計算
        currentPos.y = Mathf.SmoothDamp(
            currentPos.y,
            _playerTransform.position.y + _heightOffset,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );

        // 現在位置のx座標を更新
        transform.position = currentPos;
    }
}
