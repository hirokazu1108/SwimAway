using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField, Tooltip("押す力")] private float _power;



    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // 最も近い点を取得
        var closePoint = collision.collider.ClosestPoint(transform.position);
        var boundVec = collision.transform.position - closePoint;

        var iwashi = collision.gameObject.GetComponent<Iwashi>();
        var rb = collision.gameObject.GetComponent<Rigidbody>();

        float power = _power * rb.velocity.magnitude;
        iwashi.AddForceAndChangeDirection(boundVec.normalized, power);
    }
}
