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
    [SerializeField, Header("キー入力の受付間隔")] private float _inputInterval;

    [Space(10), Header("[State]")]
    [SerializeField] private MoveState _moveState = MoveState.MoveHorizontal;    //現在の状態
    [SerializeField] private Vector2 _potentialDirection = new Vector2(1,1);     //潜在的な方向
    [SerializeField] private float _elapsedTime = 0f;    //経過時間


    private Rigidbody _rb = null;
    private float _targetSpeed;  //目標の速さ（adjustSpeed()で管理）
    private bool _isTurning = false;
    private float _inputTimer = 99f;

    //不要
    [Space(10), Header("[確認用]")]
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

    //ユーザ入力
    private void userInput()
    {
        //キー入力間隔を適応
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

    //時間経過による速度変化
    private void adjustSpeed()
    {
        _elapsedTime += Time.deltaTime;
        _targetSpeed = Mathf.Sqrt(_elapsedTime)+3;
    }

    //進行処理
    private void move()
    {
        //if (_isTurning) return;

        // dirの向きに目標速度を維持して移動
        var power = 0f;
        if (_moveState != MoveState.Stop) power = _rb.velocity.magnitude+1;  // 加える力目標速度に到達するまでの時間を変化させられる

        var vectorAddForce = transform.right * (_targetSpeed - _rb.velocity.magnitude) * 10;
        _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
    }

    //指定した方向を向く
    //TODO:もう少し動きこだわろう、OnTriggerEnterを複数回拾う挙動がありそう
    private IEnumerator turnTo(Vector2 direction)
    {
        _head.SetActive(false);
        _isTurning = true;

        //指定方向に向くための変数のセット
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
            _rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;    //x軸方向にしか移動できないようにする
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

        _head.SetActive(true);
        _isTurning = false;

        yield break;
    }

    //反射の処理
    private void reflect()
    {
        //潜在方向の反転
        if (_moveState == MoveState.MoveHorizontal)
        {
            _potentialDirection = new Vector2(-_potentialDirection.x, _potentialDirection.y);
        }
        else if (_moveState == MoveState.MoveVertical)
        {
            _potentialDirection = new Vector2(_potentialDirection.x, -_potentialDirection.y);
        }

        //キャラクターの向きを変える処理
        StartCoroutine(turnTo(_potentialDirection));
    }

    public void addForce(Vector2 dir, float power = 0f)
    {
        var cosIdentityDirAngle = Mathf.Cos(75 * Mathf.Deg2Rad);    //縦か横かを判断する角度の閾値
        var cosVec = Vector2.Dot(Vector2.right, dir) / dir.magnitude;   // (1,0)とdirのなす角のcos値を求める  =（内積）/ (各ベクトルの大きさの積)
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

        _infoText.text = $"Time : {(int)_elapsedTime}秒\nSpeed : {_rb.velocity.magnitude}\nMode : {_moveState}\nDirection : ({_potentialDirection.x},{_potentialDirection.y})";
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isTurning) return;

        reflect();  //進行方向の反転処理
    }

}
