using UnityEngine;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [SerializeField] Text _scoreText;
    [SerializeField] Text _coinText;
    [SerializeField] PlayerSharedData _sharedData;



    private void OnEnable()
    {
        var score = 100 * Mathf.Round(_sharedData.GetDistanceSum() + 100 * _sharedData.GetCoinNum);
        _coinText.text = _sharedData.GetCoinNum.ToString("00") + "/ 57";
        _scoreText.text = score.ToString("#,0");
        
    }
    
}
