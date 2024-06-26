using UnityEngine;

public class BlackHole: MonoBehaviour
{
   private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        GameManager.GameOver();
    }
}
