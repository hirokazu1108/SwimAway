using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public enum MoveState
    {
        Stop,
        MoveHorizontal,
        MoveVertical,
    }

    // ��ԕϐ�
    [SerializeField, Tooltip("���݂̏��")] private MoveState _moveState = MoveState.MoveHorizontal;
    [SerializeField, Tooltip("���ݓI�ȕ���")] private Vector2 _potentialDirection = new Vector2(1, 1);

    // �ړ��ɗ��p
    private float _targetSpeed = 0.0f;
    private float _speedRate = 1.0f;
    private bool _isTurning = false;
    private float _cosIdentityDirAngle = 0.0f;

    // ����
    [SerializeField, Tooltip("�L�[���͂̎�t�Ԋu")] private float _inputInterval;
    private float _inputTimer = 99f;

    // �R���|�[�l���g
    private Rigidbody _rb = null;

    // getter
    public float SpeedRate => _speedRate;

    // setter
    public void setSpeedRate(float rate){
        _speedRate = Mathf.Abs(rate); 
    }


    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        SetBoundSetting();
    }

    private void Update()
    {
        AdjustSpeed();
        UserInput();
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

    /// <summary>
    /// �o�E���h�����̐ݒ�
    /// </summary>
    private void SetBoundSetting()
    {
        const float adjustingAngle = 3f * Mathf.Deg2Rad;    // �����p�̃p�����[�^�i���p�x�̋��e�j
        var colliderSize = GetComponent<BoxCollider>().size;
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
    }

    /// <summary>
    /// ���Ԍo�߂ɂ�鑬�x�ω�
    /// </summary>
    private void AdjustSpeed()
    {
        _targetSpeed = Mathf.Sqrt(GameManager.GameTime) + 1;    // ���Ԃɂ�鑬�x�ω�
        _targetSpeed *= _speedRate; // ���x�������s��
    }

    private void Move()
    {
        // dir�̌����ɖڕW���x���ێ����Ĉړ�
        var power = 0f;
        if (_moveState != MoveState.Stop) power = _rb.velocity.magnitude + 1;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

        var vectorAddForce = transform.right * (_targetSpeed - _rb.velocity.magnitude) * 10;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
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

        if (_moveState == MoveState.MoveHorizontal)
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
        else if (_moveState == MoveState.MoveVertical)
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
}
