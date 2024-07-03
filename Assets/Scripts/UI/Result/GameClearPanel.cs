using System.Buffers;
using UnityEngine;
using UnityEngine.UI;

public class GameClearPanel : MonoBehaviour
{
    [SerializeField] Text _scoreText;
    [SerializeField] PlayerSharedData _sharedData;



    private void OnEnable()
    {
        var score = Mathf.Round(_sharedData.GetDistanceSum() * (1 + (0.1f * _sharedData.GetDistanceSum())));
        _scoreText.text = score.ToString("#,0");
    }
    
}
