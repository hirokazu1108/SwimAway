using UnityEngine;

public class DeadWall : Enemy
{

    // �ړ��ɗ��p
    private float _targetSpeed;
    [SerializeField, Tooltip("�ڕW�l�ɓ��B����܂ł̂����悻�̎���[s]")] private float _smoothTime = 0.3f;
    private float _maxSpeed = float.PositiveInfinity;
    private float _currentVelocity = 0;

    // �R���|�[�l���g
    private Rigidbody _rb = null;
    private Transform _playerTransform = null;


    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
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
        GameManager.GameOver();
    }


    /// <summary>
    /// ���Ԍo�߂ɂ�鑬�x�ω�
    /// </summary>
    private void adjustSpeed()
    {
        _targetSpeed = Mathf.Sqrt(GameManager.GameTime);
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
            _playerTransform.position.y,
            ref _currentVelocity,
            _smoothTime,
            _maxSpeed
        );

        // ���݈ʒu��x���W���X�V
        transform.position = currentPos;
    }
}
