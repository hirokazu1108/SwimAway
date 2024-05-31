using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    [SerializeField] GameObject iwashi;
    [SerializeField] GameObject kajiki;
    private Image img;
    private float speed = 1.0f; // 点滅速度
    private float time;
    private float distanceOfDeath;
    private float[] dis = { 1, 3, 5 };  // 警告範囲
    void Start()
    {
        img = GetComponent<Image>();
        img.color = Color.clear;
    }

    void Update()
    {
        distanceOfDeath = iwashi.transform.position.x - kajiki.transform.position.x;
        img.color = GetImgColor(img.color);
    }

    Color GetImgColor(Color color)
    {
        // 透明度
        time += Time.deltaTime * speed * 5.0f;
        if (distanceOfDeath <= dis[0])
        {
            color.a = (Mathf.Sin(time) + 0.6f) / 2.0f;
        }
        else if(distanceOfDeath <= dis[1] && distanceOfDeath > dis[0])
        {
            color.a = Mathf.Sin(time) / 2.0f;
        }
        else if(distanceOfDeath <= dis[2] && distanceOfDeath > dis[1])
        {
            color.a = Mathf.Sin(time) / 4.0f;
        }
        else if(distanceOfDeath > dis[2])
        {
            color.a = 0;
        }

        color.r = 0.7f; // 赤色
        
        return color;
    }
}
