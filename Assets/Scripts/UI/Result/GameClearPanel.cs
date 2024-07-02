using UnityEngine;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [SerializeField] Text _scoreText;
    [SerializeField] PlayerSharedData _sharedData;

    private void Update()
    {
        _scoreText.text = $"({_sharedData.GetCoinNum})–‡  ({_sharedData.GetDistanceSum()}m)";
    }
}
