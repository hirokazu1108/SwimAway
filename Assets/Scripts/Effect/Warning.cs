using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    private GameObject iwashi;
    private GameObject kajiki;
    [SerializeField, Tooltip("�x���͈�")] private float[] dis = { 5, 10, 20 };  // �x���͈�
    private Image img;
    private float speed = 1.0f; // �_�ő��x
    private float time;
    private float distanceOfDeath;
    
    void Start()
    {
        img = GetComponent<Image>();
        img.color = Color.clear;

        iwashi = GameObject.FindGameObjectWithTag("Player");
        kajiki = GameObject.FindGameObjectWithTag("LimitWall");
        
    }

    void Update()
    {
        distanceOfDeath = iwashi.transform.position.x - kajiki.transform.position.x;
        img.color = GetImgColor(img.color);
    }

    Color GetImgColor(Color color)
    {
        // �����x
        time += Time.deltaTime * speed * 5.0f;
        if (distanceOfDeath <= dis[0])
        {
            color.a = (Mathf.Sin(time) + 0.2f) / 2.0f;
        }
        else if(distanceOfDeath <= dis[1] && distanceOfDeath > dis[0])
        {
            color.a = Mathf.Sin(time) / 3.0f;
        }
        else if(distanceOfDeath <= dis[2] && distanceOfDeath > dis[1])
        {
            color.a = Mathf.Sin(time) / 4.0f;
        }
        else if(distanceOfDeath > dis[2])
        {
            color.a = 0;
        }

        color.r = 0.3f; // �ԐF
        
        return color;
    }
}
