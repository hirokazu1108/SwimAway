using UnityEngine;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [SerializeField] Text _scoreText;
    [SerializeField] PlayerSharedData _sharedData;



    private void OnEnable()
    {
        var score = 100 * Mathf.Round(_sharedData.GetDistanceSum() + 100 * _sharedData.GetCoinNum);
        _scoreText.text = score.ToString("#,0");
    }
    
}
