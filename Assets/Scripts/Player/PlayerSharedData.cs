using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerSharedData", menuName = "ScriptableObjects/PlayerSharedData")]
public class PlayerSharedData : ScriptableObject
{
    [SerializeField, Tooltip("�R�C������")] private int _coinNum = 0;
    [SerializeField, Tooltip("�e�X�e�[�W�̐i�񂾋���")] private List<float> _maxAdvancedDistanceList = new List<float>();

    // getter
    public int GetCoinNum => _coinNum;

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    public void Reset()
    {
        
        _coinNum = 0;
        _maxAdvancedDistanceList = new List<float>();
        for (int i=0; i< StageManager.StageNum(); i++)
        {
            _maxAdvancedDistanceList.Add(0);
        }
        Debug.Log(_maxAdvancedDistanceList.Count);
    }

    /// <summary>
    /// �R�C���̎擾����
    /// </summary>
    public void GetCoin()
    {
        _coinNum++;
    }

    /// <summary>
    /// �X�e�[�W���̍ō������̍X�V
    /// </summary>
    /// <param name="dist"></param>
    public void UpdateMaxDistance(float dist)
    {
        var stage = StageManager.CurrentStageIndex();
        if (_maxAdvancedDistanceList.Count <= stage) return;
        if (stage < 0) return;

        _maxAdvancedDistanceList[stage] = dist;
    }

    public float GetDistanceSum()
    {
        var sum = 0f;
        foreach(var dist in _maxAdvancedDistanceList)
        {
            sum += dist;
        }
        return sum;
    }
}
