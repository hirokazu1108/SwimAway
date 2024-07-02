using UnityEngine;

public class InkParticle : MonoBehaviour
{
    [SerializeField, Tooltip("Octopusのスクリプト")] private Octopus _octopus;
    private void OnParticleCollision(GameObject other)
    {
        if (!other.CompareTag("Player")) return;

        var iwashi = other.gameObject.GetComponent<Iwashi>();
        iwashi.ShowerInk();
    }

    private void OnParticleSystemStopped()
    {
        StartCoroutine(_octopus.ShootInk());
    }
}
