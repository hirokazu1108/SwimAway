using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool _isAvailable = true;    // �擾�\��Ԃ�

    private void OnTriggerEnter(Collider other)
    {
        // �擾�\��ԁ@���@�v���C���[�ł���
        if (_isAvailable && other.CompareTag("Player"))
        {
            _isAvailable = false;

            Debug.Log("TODO:�R�C���̎擾����");
            Destroy(this.gameObject);
        }
    }
}
