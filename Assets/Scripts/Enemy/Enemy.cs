using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public float speed;


    public abstract void Move();

    public abstract void Hit();

    

}
