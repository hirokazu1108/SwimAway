using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public enum MoveState
    {
        Stop,
        MoveHorizontal,
        MoveVertical,
    }

    [Space(10), Header("[Attachment]")]
    [SerializeField] private GameObject _head;

    [Space(10),Header("[Parameter]")]
    [SerializeField, Header("�L�[���͂̎�t�Ԋu")] private float _inputInterval;

    [Space(10), Header("[State]")]
    [SerializeField] private MoveState _moveState = MoveState.MoveHorizontal;    //���݂̏��
    [SerializeField] private Vector2 _potentialDirection = new Vector2(1,1);     //���ݓI�ȕ���
    [SerializeField] private float _elapsedTime = 0f;    //�o�ߎ���


    private Rigidbody _rb = null;
    private float _targetSpeed;  //�ڕW�̑����iadjustSpeed()�ŊǗ��j
    private bool _isTurning = false;
    private float _inputTimer = 99f;

    //�s�v
    [Space(10), Header("[�m�F�p]")]
    [SerializeField] private GameObject _infoCanvas;
    [SerializeField] private Text _infoText;
    [SerializeField] private bool _needsInfo;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        adjustSpeed();
        userInput();
        showInfoCanvas();
    }

    private void FixedUpdate()
    {
        move();
    }

    //���[�U����
    private void userInput()
    {
        //�L�[���͊Ԋu��K��
        _inputTimer += Time.deltaTime;
        if (_inputTimer < _inputInterval) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (_moveState == MoveState.MoveHorizontal)
            {
                _moveState = MoveState.MoveVertical;
                StartCoroutine(turnTo(_potentialDirection));
                _inputTimer = 0;
            }
            else if (_moveState == MoveState.MoveVertical)
            {
                _moveState = MoveState.MoveHorizontal;
                StartCoroutine(turnTo(_potentialDirection));
                _inputTimer = 0;
            }
        }
    }

    //���Ԍo�߂ɂ�鑬�x�ω�
    private void adjustSpeed()
    {
        _elapsedTime += Time.deltaTime;
        _targetSpeed = Mathf.Sqrt(_elapsedTime)+3;
    }

    //�i�s����
    private void move()
    {
        //if (_isTurning) return;

        // dir�̌����ɖڕW���x���ێ����Ĉړ�
        var power = 0f;
        if (_moveState != MoveState.Stop) power = _rb.velocity.magnitude+1;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

        var vectorAddForce = transform.right * (_targetSpeed - _rb.velocity.magnitude) * 10;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    //�w�肵������������
    //TODO:������������������낤�AOnTriggerEnter�𕡐���E�����������肻��
    private IEnumerator turnTo(Vector2 direction)
    {
        _head.SetActive(false);
        _isTurning = true;

        //�w������Ɍ������߂̕ϐ��̃Z�b�g
        Quaternion targetRot = transform.rotation;

        if(_moveState == MoveState.MoveHorizontal)
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
        else if(_moveState == MoveState.MoveVertical)
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

        _head.SetActive(true);
        _isTurning = false;

        yield break;
    }

    //���˂̏���
    private void reflect()
    {
        //���ݕ����̔��]
        if (_moveState == MoveState.MoveHorizontal)
        {
            _potentialDirection = new Vector2(-_potentialDirection.x, _potentialDirection.y);
        }
        else if (_moveState == MoveState.MoveVertical)
        {
            _potentialDirection = new Vector2(_potentialDirection.x, -_potentialDirection.y);
        }

        //�L�����N�^�[�̌�����ς��鏈��
        StartCoroutine(turnTo(_potentialDirection));
    }

    public void addForce(Vector2 dir, float power = 0f)
    {
        var cosIdentityDirAngle = Mathf.Cos(75 * Mathf.Deg2Rad);    //�c�������𔻒f����p�x��臒l
        var cosVec = Vector2.Dot(Vector2.right, dir) / dir.magnitude;   // (1,0)��dir�̂Ȃ��p��cos�l�����߂�  =�i���ρj/ (�e�x�N�g���̑傫���̐�)
        //Debug.Log(Mathf.Acos(cosVec)*Mathf.Rad2Deg);
        if(cosIdentityDirAngle < Mathf.Abs(cosVec))
        {
            _moveState = MoveState.MoveHorizontal;
            _potentialDirection.x = dir.x / Mathf.Abs(dir.x);
        }
        else
        {
            _moveState = MoveState.MoveVertical;
            _potentialDirection.y = dir.y / Mathf.Abs(dir.y);
        }

        StartCoroutine(turnTo(_potentialDirection));
        _rb.AddForce(power * dir, ForceMode.Impulse);
    }


    private void showInfoCanvas()
    {
        if (_infoCanvas == null) return;

        _infoCanvas.SetActive(_needsInfo);

        if (!_needsInfo) return;

        _infoText.text = $"Time : {(int)_elapsedTime}�b\nSpeed : {_rb.velocity.magnitude}\nMode : {_moveState}\nDirection : ({_potentialDirection.x},{_potentialDirection.y})";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isTurning) return;

        reflect();  //�i�s�����̔��]����
    }

}
