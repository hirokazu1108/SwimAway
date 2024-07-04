using UnityEngine;

public class BlackHole: MonoBehaviour
{
    private GameManager _gameManager;
    private void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var player = other.gameObject.GetComponent<Iwashi>();
        if (player.IsInvincible) return;

        _gameManager.GameOver();
    }
}
