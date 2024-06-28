using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField, Tooltip("‰Ÿ‚·—Í")] private float _power;



    private void OnCollisionEnter(Collision collision)
    {
        
        if (!collision.gameObject.CompareTag("Player")) return;

        var iwashi = collision.gameObject.GetComponent<Iwashi>();
        iwashi.setBound(_power);
    }
}
