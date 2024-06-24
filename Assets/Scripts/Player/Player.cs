using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum MoveState
    {
        Stop,
        MoveHorizontal,
        MoveVertical,
        Invincible, // ���G���[�h
    }

    // ��ԕϐ�
    [SerializeField, Tooltip("���݂̏��")] private MoveState _moveState = MoveState.MoveHorizontal;
    [SerializeField, Tooltip("���ݓI�ȕ���")] private Vector2 _potentialDirection = new Vector2(1, 1);

    // �ړ��ɗ��p
    private float _targetSpeed = 0.0f;
    [SerializeField, Tooltip("�ő呬�x")] private float _maxSpeed;
    private float _speedRate = 1.0f;
    private bool _isTurning = false;
    private float _cosIdentityDirAngle = 0.0f;

    // ���G
    private float _invincibledElapsedTime = 0.0f;   // ���G���g���Ă���̎���
    [SerializeField, Tooltip("���G���g����܂ł̎���")] private float _invincibleCanUseTime;
    [SerializeField, Tooltip("���G����")] private float _invincibleTime;
    [SerializeField, Tooltip("���G���̑��x�{��")] private float _invincibleSpeedRate;
    [SerializeField, Tooltip("�ڕW�l�ɓ��B����܂ł̂����悻�̎���[s]")] private float _invincibleSmoothTime;
    private float _currentInvincibleVelocity = 0;

    // ����
    [SerializeField, Tooltip("�L�[���͂̎�t�Ԋu")] private float _inputInterval;
    private float _inputTimer = 99f;

    // �R���|�[�l���g
    private Rigidbody _rb = null;
    private BoxCollider _collider = null;
    [SerializeField, Tooltip("�ʏ탂�f���̃I�u�W�F�N�g")] private GameObject _normalModel;
    [SerializeField, Tooltip("���G���f���̃I�u�W�F�N�g")] private GameObject _invincibleModel;

    // getter
    public float SpeedRate => _speedRate;

    // setter
    public void setSpeedRate(float rate){
        _speedRate = Mathf.Abs(rate);
    }

    // ��Ԃ�Ԃ�����
    public bool IsInvincible => (_moveState == MoveState.Invincible);


    #region --- Unity���C�t�T�C�N�� ---

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
    /// �o�E���h�����̐ݒ�
    /// </summary>
    private void SetBoundSetting()
    {
        const float adjustingAngle = 3f * Mathf.Deg2Rad;    // �����p�̃p�����[�^�i���p�x�̋��e�j
        var colliderSize = _collider.size;
        _cosIdentityDirAngle = Mathf.Cos(Mathf.Atan(colliderSize.y / colliderSize.x) + adjustingAngle);   //�c�������𔻒f����p�x��臒l
    }

    /// <summary>
    /// ���[�U�̓��͎�t���\�b�h
    /// </summary>
    private void UserInput()
    {
        if (GameManager.IsPauseGame) return;

        //�L�[���͊Ԋu��K��
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
    /// ���Ԍo�߂ɂ�鑬�x�ω�
    /// </summary>
    private void AdjustSpeed()
    {
        _targetSpeed = Mathf.Sqrt(GameManager.GameTime) + 1;    // ���Ԃɂ�鑬�x�ω�
        
        _targetSpeed *= _speedRate; // ���x�������s��

        _targetSpeed = Mathf.Min(_targetSpeed, _maxSpeed);  // �ő呬�x�𒴂��Ȃ��悤��
    }

    private void Move()
    {
        // dir�̌����ɖڕW���x���ێ����Ĉړ�
        var power = 0f;
        if (_moveState != MoveState.Stop) power = _rb.velocity.magnitude + 1;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

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
    /// �w�肵������������
    /// </summary>
    /// <param name="direction">�������������̃x�N�g��</param>
    /// <returns></returns>
    private IEnumerator TurnTo(Vector2 direction)
    {
        _isTurning = true;

        //�w������Ɍ������߂̕ϐ��̃Z�b�g
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
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;    //x�������ɂ����ړ��ł��Ȃ��悤�ɂ���
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
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;     //y�������ɂ����ړ��ł��Ȃ��悤�ɂ���
        }

        //��]����
        var frameCount = 0;
        const float rotateFrame = 18f; //��]�ɂ�����t���[����
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
    /// ���݂̐i�s�����̃x�N�g�����擾
    /// </summary>
    /// <returns>���݂̐i�s����</returns>
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
    /// �o�E���h����
    /// </summary>
    /// <param name="collidePoint">�Փ˂������W</param>
    /// <param name="power">�o�E���h�̋���</param>
    private void Bound(Vector3 collidePoint, float power = 1.0f)
    {

        Vector2 dir = (transform.position - collidePoint).normalized;        
        var cosVec = Vector2.Dot(Vector2.right, dir) / dir.magnitude;   // (1,0)��dir�̂Ȃ��p��cos�l�����߂�  =�i���ρj/ (�e�x�N�g���̑傫���̐�)


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


    #region --- ���G�Ɋւ��鏈�� ---

        /// <summary>
        /// ���G���[�h�ɂȂ�
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
            Invoke("ExitInvincible", _invincibleTime);  // �I��������o�^
        }

        /// <summary>
        /// ���G���[�h�I��
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
        /// ���G���̍����̒���
        /// </summary>
        private void adjustHeight()
        {
            // ���݈ʒu�擾
            var currentPos = transform.position;
            var targetHeight = 0f;

            // ���t���[���̈ʒu���v�Z
            currentPos.y = Mathf.SmoothDamp(
                currentPos.y,
                targetHeight,
                ref _currentInvincibleVelocity,
                _invincibleSmoothTime,
                _maxSpeed
            );

            // ���݈ʒu��x���W���X�V
            transform.position = currentPos;
        }
    #endregion
}
