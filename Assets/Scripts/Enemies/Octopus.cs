using System.Collections;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    [SerializeField, Tooltip("発見から発射までのインターバル")] private float _shootInterval = 2.5f;
    [SerializeField, Tooltip("プレイヤーヘ向く速度")] private float _rotateSpeed = 5f;
    [SerializeField, Tooltip("プレイヤーを見失う距離")] private float _missDistance = 15f;

    [SerializeField, Tooltip("墨のエフェクト")] private ParticleSystem _octopusInkParticle;

    private GameObject _targetObject = null;
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
    /// プレイヤの方向を向く
    /// </summary>
    private void Look()
    {
        Quaternion rot = Quaternion.identity;

        if (_targetObject != null)   // プレイヤー発見中
        {
            Vector3 vector3 = (_targetObject.transform.position - this.transform.position).normalized;    // 対象物と自分自身の座標からベクトルを算出
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

    /// <summary>
    /// インクの発射処理
    /// </summary>
    /// <param name="interval">発射間隔</param>
    /// <returns></returns>
    public IEnumerator ShootInk(float interval = 1f)
    {
        if (_enableShootCortine) yield break ;
        Debug.Log("発射待機");
        _enableShootCortine = true;
        yield return new WaitForSeconds(interval);

        if (_targetObject != null)
        {
            Debug.Log("発射");
            _octopusInkParticle.Play();
        }
        _enableShootCortine = false;
        yield break;
    }

    /// <summary>
    /// プレイヤーを見失ったか
    /// </summary>
    /// <returns>trueなら見失った</returns>
    private bool CheckMissingPlayer()
    {
        if (_targetObject == null) return true;

        var dist = Vector3.Distance(transform.position, _targetObject.transform.position);

        return _missDistance < dist ? true : false;
    }

}
