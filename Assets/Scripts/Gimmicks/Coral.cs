using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coral : MonoBehaviour
{
    [SerializeField, Tooltip("‰Ÿ‚·—Í")] private float _power;
    private void OnCollisionEnter(Collision collision)
    {

        if (!collision.gameObject.CompareTag("Player")) return;

        var iwashi = collision.gameObject.GetComponent<Iwashi>();
        iwashi.setBound(_power);
    }
}
