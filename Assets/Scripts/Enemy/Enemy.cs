using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour
{
    public int speed;

    protected abstract void Init();

    public abstract void Move();

    public abstract void Hit();

    

}
