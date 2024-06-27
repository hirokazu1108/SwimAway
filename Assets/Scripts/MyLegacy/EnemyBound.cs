using UnityEngine;
using MyLegacy;

namespace MyLegacy
{
    public class EnemyBound : MonoBehaviour
    {
        // ���˕Ԃ菈��
        [SerializeField, Tooltip("���˕Ԃ��̃N�[���^�C��")] private float _boundInterval;
        [SerializeField, Tooltip("���˕Ԃ�̋���")] private float _boundPower;
        private bool _isBound = false;    //�o�E���h�����̃t���O

        // �R���|�[�l���g
        [SerializeField] private Rigidbody _rb = null;

        // getter
        public float BoundPower => _boundPower;

        private void OnCollisionEnter(Collision collision)
        {
            WaitBoundInterval();
            Bound(collision.contacts[0].point);
        }

        /// <summary>
        /// �o�E���h��̃N�[�����Ԃ�ҋ@
        /// </summary>
        private void WaitBoundInterval()
        {
            _isBound = true;
            _rb.isKinematic = false;
            Invoke("ExitBoundState", _boundInterval);
        }

        /// <summary>
        /// �o�E���h�̃N�[�����Ԃ��I��
        /// </summary>
        private void ExitBoundState()
        {
            _rb.velocity = Vector3.zero;
            _isBound = false;
            _rb.isKinematic = true;
        }

        /// <summary>
        /// �Փˎ��̒��˕Ԃ菈��
        /// </summary>
        /// <param name="collidePoint">�Փ˂������W</param>
        public void Bound(Vector3 collidePoint)
        {
            var dir = (collidePoint - transform.position).normalized;
            _rb.AddForce(-dir * _boundPower, ForceMode.Impulse);
        }
    }

}