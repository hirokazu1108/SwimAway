using UnityEngine;


[CreateAssetMenu(fileName = "PlayerSharedData", menuName = "ScriptableObjects/PlayerSharedData")]
public class PlayerSharedData : ScriptableObject
{
    [SerializeField, Tooltip("コイン枚数")] private int _coinNum = 0;
    [SerializeField, Tooltip("進んだ距離")] private float _maxAdvancedDistance = 0f;

    // getter
    public int GetCoinNum => _coinNum;

    /// <summary>
    /// リセット処理
    /// </summary>
    public void Reset()
    {
        _coinNum = 0;
        _maxAdvancedDistance = 0;
    }

    /// <summary>
    /// コインの取得処理
    /// </summary>
    public void GetCoin()
    {
        _coinNum++;
    }


}
