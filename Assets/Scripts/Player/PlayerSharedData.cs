using UnityEngine;


[CreateAssetMenu(fileName = "PlayerSharedData", menuName = "ScriptableObjects/PlayerSharedData")]
public class PlayerSharedData : ScriptableObject
{
    [SerializeField, Tooltip("�R�C������")] private int _coinNum = 0;
    [SerializeField, Tooltip("�i�񂾋���")] private float _maxAdvancedDistance = 0f;

    // getter
    public int GetCoinNum => _coinNum;

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    public void Reset()
    {
        _coinNum = 0;
        _maxAdvancedDistance = 0;
    }

    /// <summary>
    /// �R�C���̎擾����
    /// </summary>
    public void GetCoin()
    {
        _coinNum++;
    }


}