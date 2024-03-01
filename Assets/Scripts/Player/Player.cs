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

    private float _targetSpeed;  //目標の速さ（adjustSpeed()で管理）
    [SerializeField] private MoveState _moveState = MoveState.Stop;    //現在の状態
    private Directions2 _potentialDirection = new Directions2(1,1);     //潜在的な方向

    private Rigidbody _rb = null;
    private float _elapsedTime = 0f;    //経過時間

    [SerializeField, Header("キー入力の受付間隔")] private float _inputInterval;
    private float _inputTimer = 99f;

    [SerializeField, Header("Headのトリガー判定間隔")] private float _triggerInterval;
    private float _triggerTimer = 99f;

    //不要
    [Header("確認用")]
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

    //ユーザ入力
    private void userInput()
    {
        //キー入力間隔を適応
        _inputTimer += Time.deltaTime;
        if (_inputTimer < _inputInterval) return;

        if (Input.GetKeyDown(KeyCode.UpArrow) && _moveState != MoveState.MoveVertical)
        {
            if (_moveState == MoveState.Stop) _potentialDirection.setVertical(1);   //停止時

            _moveState = MoveState.MoveVertical;
            if (_moveState != MoveState.Turn) StartCoroutine(turnTo(_potentialDirection.Y));
            _rb.velocity = Vector3.zero;

            _inputTimer = 0;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow) && _moveState != MoveState.MoveHorizontal)   
        {
            if (_moveState == MoveState.Stop) _potentialDirection.setHorizontal(1); //停止時

            _moveState = MoveState.MoveHorizontal;
            if (_moveState != MoveState.Turn) StartCoroutine(turnTo(_potentialDirection.X));
            _rb.velocity = Vector3.zero;

            _inputTimer = 0;
        }
    }

    //時間経過による速度変化
    private void adjustSpeed()
    {
        _elapsedTime += Time.deltaTime;
        _targetSpeed = Mathf.Sqrt(_elapsedTime);
    }

    //進行処理
    private void move()
    {
        if (_moveState == MoveState.Turn) return;

        // dirの向きに目標速度を維持して移動
        var power = 0f;    
        if (_moveState != MoveState.Stop) power = 15f;  // 加える力目標速度に到達するまでの時間を変化させられる

        var vectorAddForce = transform.forward * (_targetSpeed - _rb.velocity.magnitude) * power;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    //指定した方向を向く
    //TODO:もう少し動きこだわろう、OnTriggerEnterを複数回拾う挙動がありそう
    private IEnumerator turnTo(Direction direction)
    {  
        //指定方向に向くための変数のセット
        Quaternion targetRot = transform.rotation;
        MoveState targetMoveState = _moveState;
        _moveState = MoveState.Turn;    //状態を回転状態に

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

        //回転処理
        var frameCount = 0;
        const float rotateFrame = 18f; //回転にかけるフレーム数
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

    //反射の処理
    private void reflect()
    {
        if (_moveState == MoveState.Turn) return;

        var dir = Direction.None;
        //潜在方向の反転
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

        //キャラクターの向きを変える処理
        StartCoroutine(turnTo(dir));
    }

    private void showInfoCanvas()
    {
        _infoCanvas.SetActive(_needsInfo);
        if (!_needsInfo) return;

        _infoText.text = $"Time : {(int)_elapsedTime}秒\nSpeed : {_rb.velocity.magnitude}\nMode : {_moveState}\nDirection : ({_potentialDirection.X},{_potentialDirection.Y})";
    }

    private void OnTriggerStay(Collider other)
    {
        //トリガーの間隔を適応
        if (_triggerTimer < _triggerInterval) return;

        reflect();  //進行方向の反転処理
        _triggerTimer = 0f;
    }
}
