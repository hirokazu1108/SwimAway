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
            other.gameObject.GetComponent<Player>()?.GetCoin();
            Destroy(this.gameObject);
        }
    }
}
