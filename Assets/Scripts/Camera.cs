using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;  //�v���C���[�̈ʒu

    private Vector3 _positionVector;   //�ʒu�x�N�g���i�J�������v���C���[�j

    void Start()
    {
        _positionVector = _playerTransform.position - transform.position;
    }

    private void Update()
    {
        transform.position = _playerTransform.position - _positionVector;
    }
}
