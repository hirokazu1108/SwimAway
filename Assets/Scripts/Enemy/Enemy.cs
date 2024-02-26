using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract class Enemy : MonoBehaviour
{
    public int speed;
    // Start is called before the first frame update
    public abstract void Move();

    public abstract void Hit();

}
