using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Octopus : MonoBehaviour
{
    [SerializeField, Tooltip("墨のエフェクト")] private ParticleSystem _octopusInkParticle;
    [SerializeField, Tooltip("発見から発射までのインターバル")] private float _shootIntervalFromFind;
    [SerializeField, Tooltip("プレイヤーヘ向く速度")] private float _rotateSpeed;

    private GameObject _tagetObject = null;
    private Quaternion _originalRotation = Quaternion.identity;


    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    private void Update()
    {
        if(!_octopusInkParticle.isPlaying)   Look();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tagetObject = other.gameObject;
            StartCoroutine(ShootInk());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tagetObject = null;
        }
    }

    private void Look()
    {
        Quaternion rot = Quaternion.identity;

        if (_tagetObject != null)   // プレイヤー発見中
        {
            Vector3 vector3 = _tagetObject.transform.position - this.transform.position;    // 対象物と自分自身の座標からベクトルを算出
            rot = Quaternion.LookRotation(vector3);    // Quaternion(回転値)を取得
        }
        else
        {
            rot = _originalRotation;
        }

        rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * _rotateSpeed);
        this.transform.rotation = rot;
    }

    private IEnumerator ShootInk()
    {
        yield return new WaitForSeconds(_shootIntervalFromFind);
        if(_tagetObject != null)    _octopusInkParticle.Play();
        yield break;
    }

}
