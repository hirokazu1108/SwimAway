using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Octopus : MonoBehaviour
{
    [SerializeField,Tooltip("墨のエフェクト")] private ParticleSystem _octopusInkParticle;

    private GameObject _tagetObject = null;

    private void Update()
    {
        if (_tagetObject != null)
        {
            Vector3 vector3 = _tagetObject.transform.position - this.transform.position;    // 対象物と自分自身の座標からベクトルを算出
            // もし上下方向の回転はしない(Baseオブジェクトが床から離れないようにする)ようにしたければ以下のようにする。
            // vector3.y = 0f;

            Quaternion quaternion = Quaternion.LookRotation(vector3);    // Quaternion(回転値)を取得
            this.transform.rotation = quaternion;
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tagetObject = other.gameObject;
            _octopusInkParticle.Play();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tagetObject = null;
        }
    }

}
