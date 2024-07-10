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
    
    private float imageSpeed = 0.001f;
    private float textSpeed = 0.003f;

    private float[] imageColor = new float[4];
    private float[] textColor = new float[4];

    public bool imageFade = false;
    public bool textFade = false;

    // Start is called before the first frame update
    void Start()
    {
        imageColor[0] = fadeImage.color.r;
        imageColor[1] = fadeImage.color.g;
        imageColor[2] = fadeImage.color.b;
        imageColor[3] = fadeImage.color.a;

        textColor[0] = stageText.color.r;
        textColor[1] = stageText.color.g;
        textColor[2] = stageText.color.b;
        textColor[3] = stageText.color.a;

        //imageFade = true;
        textFade = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (imageFade)
        {
            ImageFade();
        }

        if(textFade)
        {
            TextFade();
        }
    }

    private void ImageFade()
    {
        imageColor[3] -= imageSpeed;
        Alpha();
        if(imageColor[3] <= 0)
        {
            imageFade = false;
            fadeImage.enabled = false;
        }
    }

    private void TextFade()
    {
        textColor[3] -= textSpeed;
        Alpha();
        if (textColor[3] <= 0)
        {
            textFade = false;
            stageText.enabled = false;
        }
    }

    private void Alpha()
    {
        fadeImage.color = new Color(imageColor[0], imageColor[1], imageColor[2], imageColor[3]);
        stageText.color = new Color(textColor[0], textColor[1], textColor[2], textColor[3]);
    }
}
