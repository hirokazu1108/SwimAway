using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    [SerializeField] Image warningImg;
    [SerializeField] GameObject Iwashi;
    [SerializeField] GameObject Kajiki;
    float distanceOfDeath;
    float[] dis = { 1, 3, 5 };    // åxçêÇ™î≠ê∂Ç∑ÇÈãóó£
    float[] alph = { 0.8f, 0.5f, 0.3f };    // âÊñ ÇÃîZÇ≥
    void Start()
    {
        warningImg.color = Color.clear;
    }

    void Update()
    {
        warningImg.color = Color.Lerp(warningImg.color, Color.clear, Time.deltaTime);
        distanceOfDeath = Vector3.Distance(Iwashi.transform.position, Kajiki.transform.position);
        Approach();
    }

    void Approach()
    {

        if (distanceOfDeath < dis[0])
        {
            warningImg.color = new Color(0.7f, 0, 0, alph[0]);
        }
        else if (distanceOfDeath < dis[1] && distanceOfDeath >= dis[0])
        {
            warningImg.color = new Color(0.7f, 0, 0, alph[1]);
        }
        else if (distanceOfDeath < dis[2] && distanceOfDeath >= dis[1])
        {
            warningImg.color = new Color(0.7f, 0, 0, alph[2]);
        }
    }
}
