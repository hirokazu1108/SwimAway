using UnityEngine;
using MyLegacy;

namespace MyLegacy
{
    public class EnemyBound : MonoBehaviour
    {
        // 跳ね返り処理
        [SerializeField, Tooltip("跳ね返り後のクールタイム")] private float _boundInterval;
        [SerializeField, Tooltip("跳ね返りの強さ")] private float _boundPower;
        private bool _isBound = false;    //バウンド中かのフラグ

        // コンポーネント
        [SerializeField] private Rigidbody _rb = null;

        // getter
        public float BoundPower => _boundPower;

        private void OnCollisionEnter(Collision collision)
        {
            WaitBoundInterval();
            Bound(collision.contacts[0].point);
        }

        /// <summary>
        /// バウンド後のクール時間を待機
        /// </summary>
        private void WaitBoundInterval()
        {
            _isBound = true;
            _rb.isKinematic = false;
            Invoke("ExitBoundState", _boundInterval);
        }

        /// <summary>
        /// バウンドのクール時間を終了
        /// </summary>
        private void ExitBoundState()
        {
            _rb.velocity = Vector3.zero;
            _isBound = false;
            _rb.isKinematic = true;
        }

        /// <summary>
        /// 衝突時の跳ね返り処理
        /// </summary>
        /// <param name="collidePoint">衝突した座標</param>
        public void Bound(Vector3 collidePoint)
        {
            var dir = (collidePoint - transform.position).normalized;
            _rb.AddForce(-dir * _boundPower, ForceMode.Impulse);
        }
    }

}