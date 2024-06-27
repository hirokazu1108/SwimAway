using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    [SerializeField, Tooltip("âüÇ∑óÕ")] private float _power;



    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;

        // ç≈Ç‡ãﬂÇ¢ì_ÇéÊìæ
        var closePoint = collision.collider.ClosestPoint(transform.position);
        var boundVec = collision.transform.position - closePoint;

        var iwashi = collision.gameObject.GetComponent<Iwashi>();
        var rb = collision.gameObject.GetComponent<Rigidbody>();

        float power = _power * rb.velocity.magnitude;
        iwashi.AddForceAndChangeDirection(boundVec.normalized, power);
    }
}
