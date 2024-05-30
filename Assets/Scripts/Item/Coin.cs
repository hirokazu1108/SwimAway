using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool _isAvailable = true;    // 取得可能状態か

    private void OnTriggerEnter(Collider other)
    {
        // 取得可能状態　かつ　プレイヤーである
        if (_isAvailable && other.CompareTag("Player"))
        {
            _isAvailable = false;

            Debug.Log("TODO:コインの取得処理");
            Destroy(this.gameObject);
        }
    }
}
