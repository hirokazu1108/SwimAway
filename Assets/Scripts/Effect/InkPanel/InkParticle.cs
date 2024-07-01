using UnityEngine;

public class InkParticle : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        var iwashi = other.gameObject.GetComponent<Iwashi>();
        iwashi.ShowerInk();
    }
}
