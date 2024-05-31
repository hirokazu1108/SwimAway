using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{ 
    private float speed = 1.0f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            this.transform.position = new Vector3(this.transform.position.x + speed, this.transform.position.y, this.transform.position.z);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            this.transform.position = new Vector3(this.transform.position.x - speed, this.transform.position.y, this.transform.position.z);
        }
    }
}
