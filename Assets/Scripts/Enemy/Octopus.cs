using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Octopus : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleObject_OctopusInk;


    private void OnTriggerEnter(Collider other)
    {
        _particleObject_OctopusInk.Play();
    }
}
