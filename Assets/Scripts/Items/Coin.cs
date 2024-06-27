using UnityEngine;

public class Coin : MonoBehaviour
{
    private bool _isAvailable = true;    // 取得可能状態か

    void Update()
    {
        transform.Rotate(new Vector3(0, 90, 0) * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 取得可能状態　かつ　プレイヤーである
        if (_isAvailable && other.CompareTag("Player"))
        {
            _isAvailable = false;
            other.gameObject.GetComponent<Iwashi>()?.GetCoin();
            Destroy(this.gameObject);
        }
    }
}
