using System.Collections;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    [SerializeField, Tooltip("発見から発射までのインターバル")] private float _shootInterval = 2.5f;
    [SerializeField, Tooltip("プレイヤーヘ向く速度")] private float _rotateSpeed = 5f;
    [SerializeField, Tooltip("プレイヤーを見失う距離")] private float _missDistance = 15f;

    [SerializeField, Tooltip("墨のエフェクト")] private ParticleSystem _octopusInkParticle;

    private GameObject _tagetObject = null;
    private Quaternion _originalRotation;
    private bool _enableShootCortine = false; // インク発射のコルーチンが発行されているか


    private void Start()
    {
        _originalRotation = transform.rotation;
    }

    private void Update()
    {
        if(!_octopusInkParticle.isPlaying)   Look();

        if (CheckMissingPlayer())
        {
            _tagetObject = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _tagetObject = other.gameObject;
            StartCoroutine(ShootInk(_shootInterval));
        }
    }


    private void Look()
    {
        Quaternion rot = Quaternion.identity;

        if (_tagetObject != null)   // プレイヤー発見中
        {
            Vector3 vector3 = (_tagetObject.transform.position - this.transform.position).normalized;    // 対象物と自分自身の座標からベクトルを算出
            //vector3.z = 0;  // z軸で回転
            rot = Quaternion.LookRotation(vector3);    // Quaternion(回転値)を取得
        }
        else
        {
            rot = _originalRotation;
            rot.x = 0;  // x軸での回転を防ぐ
        }

        rot = Quaternion.Slerp(this.transform.rotation, rot, Time.deltaTime * _rotateSpeed);
        this.transform.rotation = rot;
    }

    private IEnumerator ShootInk(float interval = 1f)
    {
        yield return new WaitForSeconds(interval);
        if(_tagetObject != null)    _octopusInkParticle.Play();
        yield break;
    }

    /// <summary>
    /// プレイヤーを見失ったか
    /// </summary>
    /// <returns>trueなら見失った</returns>
    private bool CheckMissingPlayer()
    {
        if (_tagetObject == null) return true;

        var dist = Vector3.Distance(transform.position, _tagetObject.transform.position);

        return _missDistance < dist ? true : false;
    }

}
