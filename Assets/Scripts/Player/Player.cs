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
        Turn,
    }

    private float _targetSpeed;  //�ڕW�̑����iadjustSpeed()�ŊǗ��j
    [SerializeField] private MoveState _moveState = MoveState.Stop;    //���݂̏��
    private Directions2 _potentialDirection = new Directions2(1,1);     //���ݓI�ȕ���

    private Rigidbody _rb = null;
    private float _elapsedTime = 0f;    //�o�ߎ���

    [SerializeField, Header("�L�[���͂̎�t�Ԋu")] private float _inputInterval;
    private float _inputTimer = 99f;

    [SerializeField, Header("Head�̃g���K�[����Ԋu")] private float _triggerInterval;
    private float _triggerTimer = 99f;

    //�s�v
    [Header("�m�F�p")]
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

        _triggerTimer += Time.deltaTime;

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

        if (Input.GetKeyDown(KeyCode.UpArrow) && _moveState != MoveState.MoveVertical)
        {
            if (_moveState == MoveState.Stop) _potentialDirection.setVertical(1);   //��~��

            _moveState = MoveState.MoveVertical;
            if (_moveState != MoveState.Turn) StartCoroutine(turnTo(_potentialDirection.Y));
            _rb.velocity = Vector3.zero;

            _inputTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && _moveState != MoveState.MoveHorizontal)   
        {
            if (_moveState == MoveState.Stop) _potentialDirection.setHorizontal(1); //��~��

            _moveState = MoveState.MoveHorizontal;
            if (_moveState != MoveState.Turn) StartCoroutine(turnTo(_potentialDirection.X));
            _rb.velocity = Vector3.zero;

            _inputTimer = 0;
        }
    }

    //���Ԍo�߂ɂ�鑬�x�ω�
    private void adjustSpeed()
    {
        _elapsedTime += Time.deltaTime;
        _targetSpeed = Mathf.Sqrt(_elapsedTime);
    }

    //�i�s����
    private void move()
    {
        if (_moveState == MoveState.Turn) return;

        // dir�̌����ɖڕW���x���ێ����Ĉړ�
        var power = 0f;    
        if (_moveState != MoveState.Stop) power = 15f;  // ������͖ڕW���x�ɓ��B����܂ł̎��Ԃ�ω���������

        var vectorAddForce = transform.forward * (_targetSpeed - _rb.velocity.magnitude) * power;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    //�w�肵������������
    //TODO:������������������낤�AOnTriggerEnter�𕡐���E�����������肻��
    private IEnumerator turnTo(Direction direction)
    {  
        //�w������Ɍ������߂̕ϐ��̃Z�b�g
        Quaternion targetRot = transform.rotation;
        MoveState targetMoveState = _moveState;
        _moveState = MoveState.Turn;    //��Ԃ���]��Ԃ�

        switch (direction)
        {
            case Direction.Forward:
                targetRot = Quaternion.Euler(0, 0, 0);  //Forward
                targetMoveState = MoveState.MoveHorizontal;
                break;
            case Direction.Back:
                targetRot = Quaternion.Euler(0, 180, 0); //Back
                targetMoveState = MoveState.MoveHorizontal;
                break;
            case Direction.Up:
                targetRot = Quaternion.Euler(-90, 0, 0);  //Up
                targetMoveState = MoveState.MoveVertical;
                break;
            case Direction.Down:
                targetRot = Quaternion.Euler(90, 0, 0);  //Down
                targetMoveState = MoveState.MoveVertical;
                break;
        }

        //��]����
        var frameCount = 0;
        const float rotateFrame = 18f; //��]�ɂ�����t���[����
        while (frameCount < 180f/rotateFrame)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, rotateFrame);
            frameCount++;
            yield return new WaitForEndOfFrame();
        }

        transform.rotation = targetRot;
        _moveState = targetMoveState;

        yield break;
    }

    //���˂̏���
    private void reflect()
    {
        if (_moveState == MoveState.Turn) return;

        var dir = Direction.None;
        //���ݕ����̔��]
        if (_moveState == MoveState.MoveHorizontal)
        {
           _potentialDirection.inverseX();
            dir = _potentialDirection.X;
        }
        else if(_moveState == MoveState.MoveVertical)
        {
            _potentialDirection.inverseY();
            dir = _potentialDirection.Y;
        }

        //�L�����N�^�[�̌�����ς��鏈��
        StartCoroutine(turnTo(dir));
    }

    private void showInfoCanvas()
    {
        _infoCanvas.SetActive(_needsInfo);
        if (!_needsInfo) return;

        _infoText.text = $"Time : {(int)_elapsedTime}�b\nSpeed : {_rb.velocity.magnitude}\nMode : {_moveState}\nDirection : ({_potentialDirection.X},{_potentialDirection.Y})";
    }

    private void OnTriggerStay(Collider other)
    {
        //�g���K�[�̊Ԋu��K��
        if (_triggerTimer < _triggerInterval) return;

        reflect();  //�i�s�����̔��]����
        _triggerTimer = 0f;
    }
}
