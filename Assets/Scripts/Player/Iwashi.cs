using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Iwashi : MonoBehaviour
{
    public enum MoveState
    {
        Stop,
        Horizontal,
        Vertical,
    }


    #region --- �t�B�[���h�ϐ��Agetter�Asetter ---

    // ��ԕϐ�
    [SerializeField, Tooltip("���݂̏��")] private MoveState _moveState = MoveState.Horizontal;
    [SerializeField, Tooltip("���ݓI�ȕ���")] private Vector3 _potentialDirection = new Vector3(1, 1, 0);

    // ���W�E�ʒu
    private Vector3 _spawnPos = Vector3.zero;
    private float _maxDistance = 0f;

    // �ړ��ɗ��p
    private float _targetSpeed = 0.0f;
    [SerializeField, Tooltip("�ő呬�x")] private float _maxSpeed = 10;
    private float _speedRate = 1.0f;
    [SerializeField, Tooltip("�ڕW���x�ɒB���邽�߂̗͂̑傫���i���i�́H�j"),Range(0, 10)] private float _drivePower = 5f;  // ���i��
    [SerializeField]private float _currentDrivePower = 0;

    //�Փ˔���
    private const float _COLLIDER_ASPECT_RATIO = 1 / 2.75f; // ���킵�̃R���C�_�[�̏c����  collider.scale.y / collider.scale.x

    // ���G
    private bool _isInvincible = false;
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

    // UI
    [SerializeField, Tooltip("���G�Q�[�W��UI")] private InvGage _invGage;
    [SerializeField, Tooltip("�n�p�l���̃I�u�W�F�N�g")] private InkPanel _inkPanel;

    // ���U���g�f�[�^
    [SerializeField, Tooltip("�v���C���[�̃V�[�����L�f�[�^")] private PlayerSharedData _sharedData;


    // getter
    public float SpeedRate => _speedRate;
    public bool IsInvincible => _isInvincible;

    // setter
    public void setSpeedRate(float rate)
    {
        _speedRate = Mathf.Abs(rate);
    }

    #endregion

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

    #region --- Unity���C�t�T�C�N�� ---

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _collider = GetComponent<BoxCollider>();
        _currentDrivePower = _drivePower;
        _spawnPos = transform.position;
    }

    private void Update()
    {
        if (this.IsInvincible) AdjustHeight();

        AdjustSpeed();
        UserInput();
        InvincibleTimer();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            ShowDebugLog();
        }
    }

    private void FixedUpdate()
    {
        Move();
        MeasureDistance();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �ł��߂��_���擾
        var closePoint = collision.collider.ClosestPoint(transform.position);
        var boundVec = transform.position - closePoint;

        StartCoroutine(EvaluateDrivePower());        
        AddForceAndChangeDirection(boundVec.normalized);
    }
    #endregion


    /// <summary>
    /// ���[�U�̓��͎�t���\�b�h
    /// </summary>
    private void UserInput()
    {
        if (GameManager.IsPauseGame) return;

        //�L�[���͊Ԋu��K��
        _inputTimer += Time.deltaTime;
        if (_inputTimer < _inputInterval) return;
        if (IsInvincible) return;

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            SwitchMoveState();
            AddForceAndChangeDirection(GetCurrentDir());
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift) || Input.GetMouseButtonDown(1))
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
        if (_maxSpeed < _rb.velocity.magnitude)
        {
            _rb.velocity = _rb.velocity.normalized * _maxSpeed;
        }
        else
        {
            var vectorAddForce = GetCurrentDir() * (_targetSpeed - _rb.velocity.magnitude) * _currentDrivePower;
            _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
        }
    }

    /// <summary>
    /// �͂������Đi�s������ύX
    /// </summary>
    /// <param name="force"></param>
    public void AddForceAndChangeDirection(Vector3 dir, float power = 1)
    {
        _rb.velocity = Vector3.zero;
        ChangeAdvanceDirection(dir.normalized);
        _rb.AddForce(GetCurrentDir()*power, ForceMode.Impulse);
    }


    /// <summary>
    /// �i�s�����̕ύX
    /// </summary>
    /// <param name="dir">��������</param>
    private void ChangeAdvanceDirection(Vector3 dir)
    {
        dir = dir.normalized;
        var cos = Vector3.Dot(dir, Vector3.right);
        var cosBase = Mathf.Cos(Mathf.Atan(_COLLIDER_ASPECT_RATIO));    // ��l�@���킵�̃R���C�_�[�̏c���䂩��p�x�𒊏o  theta = arctan(collider.scale.y / collider.scale.x) 

        if (Mathf.Abs(cos) < Mathf.Abs(cosBase))
        {
            // �c����
            _potentialDirection.y = dir.y < 0 ? -1 : 1;
            _moveState = MoveState.Vertical;
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;    //y�������ɂ����ړ��ł��Ȃ��悤�ɂ���
        }
        else
        {
            // ������
            _potentialDirection.x = dir.x < 0 ? -1 : 1;
            _moveState = MoveState.Horizontal;
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;    //x�������ɂ����ړ��ł��Ȃ��悤�ɂ���
        }

        transform.rotation = Quaternion.Euler(ConvertDirectionToEuler(GetCurrentDir()));
    }

    /// <summary>
    /// �i�s�����̒P�ʃx�N�g������I�C���[�p�ɕϊ�
    /// </summary>
    /// <param name="dir">�i�s�����̒P�ʃx�N�g��</param>
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

    /// <summary>
    /// ���i�͂̌v�Z
    /// </summary>
    /// <param name="decreseRatio">�ǂꂾ�������i�߂�͂����������邩</param>
    /// <returns></returns>
    public IEnumerator EvaluateDrivePower()
    {
        var target = _drivePower;
        _currentDrivePower = _drivePower / 2;

        while (true)
        {
            if (Mathf.Abs(_currentDrivePower - _drivePower) < 1e-2)
            {
                break;
            }

            _currentDrivePower += 0.01f;

            yield return null;
        }

        yield break;
    }

    /// <summary>
    /// �O����i�s�����ɗ͂�������
    /// </summary>
    /// <param name="power"></param>
    public void setBound(float power) 
    {
        _rb.AddForce(GetCurrentDir()*power, ForceMode.Impulse);
    }

    #region --- ���G�Ɋւ��鏈�� ---

    /// <summary>
    /// ���G���[�h�ɂȂ�
    /// </summary>
    private void EnterInvincible()
    {
        if (this.IsInvincible) return;
        if (_invincibledElapsedTime < _invincibleCanUseTime) return;

        _invGage?.SetInvincibleVisible(true);
        _isInvincible = true;
        _collider.isTrigger = true;
        _normalModel.SetActive(false);
        _invincibleModel.SetActive(true);
        setSpeedRate(_speedRate * _invincibleSpeedRate);
        AddForceAndChangeDirection(Vector3.right);
        Invoke("ExitInvincible", _invincibleTime);  // �I��������o�^
    }

    /// <summary>
    /// ���G���[�h�I��
    /// </summary>
    private void ExitInvincible()
    {
        _invGage?.SetInvincibleVisible(false);
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

        // ���G�g�p�\�@���� ���G����Ȃ�
        if (_invincibleCanUseTime < _invincibledElapsedTime && !_isInvincible){
            _invGage.StartFlash();
        }
    }

    /// <summary>
    /// ���G���̍����̒���
    /// </summary>
    private void AdjustHeight()
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


    #region --- �R�C���̎擾���� ---
    public void GetCoin()
    {
        _sharedData.GetCoin();
    }

    #endregion

    #region --- �C���N�̏��� ---
    public void ShowerInk()
    {
        _inkPanel.StartAnim();
    }

    #endregion

    #region --- �����̌v������ ---
    public void MeasureDistance()
    {
        var dist = Vector3.Distance(_spawnPos, transform.position);
        if(_maxDistance < dist)
        {
            _maxDistance = dist;
        }
        
    }

    #endregion


    #region --- �f�o�b�O�p�̏��� ---

    private List<Vector3> _debugDrawList = new List<Vector3>();

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        foreach (var d in _debugDrawList)
        {

            Gizmos.DrawSphere(d, 0.1f);
        }

    }

    public void ShowDebugLog()
    {
        Debug.Log("----- Debug Show -----");

        Debug.Log($"�R�C�������F{_sharedData.GetCoinNum}");
        Debug.Log($"�ō����B�����F{_maxDistance}");

        Debug.Log("----- End -----");
    }

    #endregion

}
