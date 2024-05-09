using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Squid : MonoBehaviour
{
    [SerializeField] private GameObject _particleObject_SquidInk;
    
    private void OnCollisionEnter(Collision collision)
    {
        var obj = Instantiate(_particleObject_SquidInk, transform);
    }
}
