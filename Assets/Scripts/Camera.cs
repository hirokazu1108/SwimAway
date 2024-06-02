using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    private Transform _playerTransform;  //プレイヤーの位置

    private Vector3 _offset;   //位置ベクトル（カメラ→プレイヤー）

    void Start()
    {
        _playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        _offset = _playerTransform.position - transform.position;
    }

    private void Update()
    {
        transform.position = _playerTransform.position - _offset;
    }
}
