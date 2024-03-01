using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;  //プレイヤーの位置

    private Vector3 _positionVector;   //位置ベクトル（カメラ→プレイヤー）

    void Start()
    {
        _positionVector = _playerTransform.position - transform.position;
    }

    private void Update()
    {
        transform.position = _playerTransform.position - _positionVector;
    }
}
