using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Fade : MonoBehaviour
{
    [SerializeField] 
    private Image fadeImage;
    [SerializeField]
    private TextMeshProUGUI stageText;
    
    private float Speed = 0.001f;
    private float Speed2 = 0.005f;
    private float red, green, blue, alfa, alfaText;

    public bool fadeOut = false;
    public bool fadeIn = false;

    // Start is called before the first frame update
    void Start()
    {
        red = fadeImage.color.r;
        green = fadeImage.color.g;
        blue = fadeImage.color.b;
        alfa = fadeImage.color.a;

        alfaText = stageText.color.a;

        fadeIn = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (fadeIn)
        {
            FadeIn();
        }

        if (fadeOut)
        {
            FadeOut();
        }
    }

    void FadeIn()
    {
        alfa -= Speed;
        alfaText -= Speed2;
        Alpha();
        if(alfa <= 0)
        {
            fadeIn = false;
            fadeImage.enabled = false;
        }
    }

    void FadeOut()
    {
        fadeImage.enabled = true;
        alfa += Speed;
        alfaText += Speed2;
        Alpha();
        if(alfa >= 1)
        {
            fadeOut = false;
        }
    }

    void Alpha()
    {
        fadeImage.color = new Color(red, green, blue, alfa);
        stageText.color = new Color(255, 255, 255, alfaText);
    }
}
