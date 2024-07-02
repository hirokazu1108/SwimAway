using System.Collections;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    [SerializeField, Tooltip("�������甭�˂܂ł̃C���^�[�o��")] private float _shootInterval = 2.5f;
    [SerializeField, Tooltip("�v���C���[�w�������x")] private float _rotateSpeed = 5f;
    [SerializeField, Tooltip("�v���C���[������������")] private float _missDistance = 15f;

    [SerializeField, Tooltip("�n�̃G�t�F�N�g")] private ParticleSystem _octopusInkParticle;

    private GameObject _targetObject = null;
    private Quaternion _originalRotation;
    private bool _enableShootCortine = false; // �C���N���˂̃R���[�`�������s����Ă��邩


    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    private void Update()
    {
        if(!_octopusInkParticle.isPlaying)   Look();

        if (CheckMissingPlayer())
        {
            _targetObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (_targetObject != null) return;
        
        _targetObject = other.gameObject;
        StartCoroutine(ShootInk(_shootInterval));
        
    }

    /// <summary>
    /// �v���C���̕���������
    /// </summary>
    private void Look()
    {
        Quaternion rot = Quaternion.identity;

        if (_targetObject != null)   // �v���C���[������
        {
            Vector3 vector3 = (_targetObject.transform.position - this.transform.position).normalized;    // �Ώە��Ǝ������g�̍��W����x�N�g�����Z�o
            //vector3.z = 0;  // z���ŉ�]
            rot = Quaternion.LookRotation(vector3);    // Quaternion(��]�l)���擾
        }
        else
        {
            rot = _originalRotation;
            rot.x = 0;  // x���ł̉�]��h��
        }

        rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * _rotateSpeed);
        this.transform.rotation = rot;
    }

    /// <summary>
    /// �C���N�̔��ˏ���
    /// </summary>
    /// <param name="interval">���ˊԊu</param>
    /// <returns></returns>
    public IEnumerator ShootInk(float interval = 1f)
    {
        if (_enableShootCortine) yield break ;
        Debug.Log("���ˑҋ@");
        _enableShootCortine = true;
        yield return new WaitForSeconds(interval);

        if (_targetObject != null)
        {
            Debug.Log("����");
            _octopusInkParticle.Play();
        }
        _enableShootCortine = false;
        yield break;
    }

    /// <summary>
    /// �v���C���[������������
    /// </summary>
    /// <returns>true�Ȃ猩������</returns>
    private bool CheckMissingPlayer()
    {
        if (_targetObject == null) return true;

        var dist = Vector3.Distance(transform.position, _targetObject.transform.position);

        return _missDistance < dist ? true : false;
    }

}
