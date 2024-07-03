using UnityEngine;

public class DeadWall : Enemy
{

    // �ʒu
    [SerializeField, Tooltip("�����̃I�t�Z�b�g")] private float _heightOffset;

    // �ړ��ɗ��p
    private float _targetSpeed;
    [SerializeField, Tooltip("�ڕW�l�ɓ��B����܂ł̂����悻�̎���[s]")] private float _smoothTime;
    private float _maxSpeed = float.PositiveInfinity;
    private float _currentVelocity = 0;

    // �R���|�[�l���g
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
        float power = 10;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

        var vectorAddForce = transform.forward * (_targetSpeed - _rb.velocity.magnitude) * power;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    public override void Hit()
    {
        _gameManager.GameOver();
    }


    /// <summary>
    /// ���Ԍo�߂ɂ�鑬�x�ω�
    /// </summary>
    private void adjustSpeed()
    {
        _targetSpeed = Mathf.Sqrt(_gameManager.GameTime);
    }

    /// <summary>
    /// �v���C���[�ʒu�ɔ����������̒���
    /// </summary>
    private void adjustHeight()
    {
        // ���݈ʒu�擾
        var currentPos = transform.position;

        // ���t���[���̈ʒu���v�Z
        currentPos.y = Mathf.SmoothDamp(
            currentPos.y,
            _playerTransform.position.y + _heightOffset,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );

        // ���݈ʒu��x���W���X�V
        transform.position = currentPos;
    }
}
