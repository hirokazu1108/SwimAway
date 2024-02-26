using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class limitWall : Enemy
{
    // Start is called before the first frame update
    Vector3 position;
    void Start()
    {
        
    }

    // Update is called once per frame
    public override void Move(){
        transform.position += new Vector3(0, 0, 1) * Time.deltaTime;
    }

    public override void Hit(){
        Debug.Log("しゅーりょー");
    }
    void OnCollisionEnter(Collision collision)
    {
        Hit();
    }
}
