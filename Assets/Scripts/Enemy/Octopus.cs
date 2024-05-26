using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Octopus : MonoBehaviour
{
    [SerializeField,Tooltip("�n�̃G�t�F�N�g")] private ParticleSystem _octopusInkParticle;

    private GameObject _tagetObject = null;

    private void Update()
    {
        if (_tagetObject != null)
        {
            Vector3 vector3 = _tagetObject.transform.position - this.transform.position;    // �Ώە��Ǝ������g�̍��W����x�N�g�����Z�o
            // �����㉺�����̉�]�͂��Ȃ�(Base�I�u�W�F�N�g�������痣��Ȃ��悤�ɂ���)�悤�ɂ�������Έȉ��̂悤�ɂ���B
            // vector3.y = 0f;

            Quaternion quaternion = Quaternion.LookRotation(vector3);    // Quaternion(��]�l)���擾
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
