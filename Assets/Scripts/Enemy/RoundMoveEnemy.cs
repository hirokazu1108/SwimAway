using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoundMoveEnemy : MonoBehaviour
{
    [SerializeField, Tooltip("���񂷂�|�C���g���W�߂��e�I�u�W�F�N�g")] private Transform _roundPoints = null;

    // �ړ��ɗ��p
    [SerializeField, Tooltip("�ڕW���x")] private float _targetSpeed = 5.0f;
    [SerializeField, Tooltip("�ő呬�x")] private float _maxSpeed = 10;
    [SerializeField, Tooltip("�ڕW���x�ɒB���邽�߂̗͂̑傫��"), Range(0, 10)] private float _reachPower = 5f;
    private const float _REACHED_DISTANNCE = 1f;
    private float _currentReachPower = 0;
    [SerializeField] private List<Vector3> _roundPointList = new List<Vector3>();
    [SerializeField] private Vector3 _targetPoint = Vector3.zero;
    private int _targetIndex = 0;

    // �R���|�[�l���g
    private Rigidbody _rb = null;


    #region --- Unity���C�t�T�C�N�� ---
        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _currentReachPower = _reachPower;
            GenerateRoundPointList();
            //Destroy(_roundPoints.gameObject);
            DefineNextTargetPoint();
        }

        private void FixedUpdate()
        {
            if (IsReachedTargetPoint())
            {
                DefineNextTargetPoint();
            }
            else
            {
                Move();
            }
        }

    #endregion

    /// <summary>
    /// ���񂷂���W���X�g�𐶐�����
    /// </summary>
    private void GenerateRoundPointList()
    {

        foreach (Transform roundPoint in _roundPoints) {
            _roundPointList.Add(roundPoint.position);
        }

        if(_roundPointList.Count == 0)
        {
            _roundPointList = null;
            Debug.LogError("���񂷂���W���擾�ł��܂���ł���");
        }
    }

    /// <summary>
    /// ���̖ڕW�n�_�����߂�
    /// </summary>
    private void DefineNextTargetPoint()
    {
        if (_targetIndex < _roundPointList.Count)
        {
            _targetPoint = _roundPointList[_targetIndex++];
        }
        else
        {
            _targetIndex = 0;
        }
    }

    /// <summary>
    /// �ڕW�n�_�ɓ��B������
    /// </summary>
    /// <returns>�ڕW�n�_�ɓ��B�����Ȃ�true</returns>
    private bool IsReachedTargetPoint()
    {
        var dist = (_targetPoint - transform.position).magnitude;
        return dist < _REACHED_DISTANNCE;
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    private void Move()
    {
        var dir = (_targetPoint - transform.position).normalized;    // �ڕW�n�_�ւ̕����x�N�g��
        
        if (_maxSpeed < _rb.velocity.magnitude)
        {
            _rb.velocity = dir.normalized * _maxSpeed;
        }
        else
        {
            var vectorAddForce = dir * (_targetSpeed - _rb.velocity.magnitude) * _currentReachPower;
            _rb.AddForce(vectorAddForce, ForceMode.Acceleration);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(EvaluateReachPower());
    }

    private IEnumerator EvaluateReachPower()
    {
        var target = _reachPower;
        _currentReachPower = _reachPower / 2;

        while (true)
        {
            if (Mathf.Abs(_currentReachPower - _reachPower) < 1e-2)
            {
                break;
            }

            _currentReachPower += 0.01f;

            yield return null;
        }

        yield break;
    }
}
