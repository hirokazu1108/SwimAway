using UnityEngine;
using UnityEngine.UI;

public class Warning : MonoBehaviour
{
    private GameObject iwashi;
    private GameObject kajiki;
    [SerializeField, Tooltip("警告範囲")] private float[] dis = { 5, 10, 20 };  // 警告範囲
    private Image img;
    private float speed = 1.0f; // 点滅速度
    private float time;
    private float distanceOfDeath;

    private int warningFlag = 0;
    private bool musicflag = false;
    //music
    private WarningMusic music;

    void Start()
    {
        img = GetComponent<Image>();
        img.color = Color.clear;

        iwashi = GameObject.FindGameObjectWithTag("Player");
        kajiki = GameObject.FindGameObjectWithTag("LimitWall");
        music = GameObject.Find("Warning").GetComponent<WarningMusic>();
        
       
    }

    void Update()
    {
        distanceOfDeath = iwashi.transform.position.x - kajiki.transform.position.x;
        img.color = GetImgColor(img.color);
        
        if (warningFlag == 1){
             music.startMusic();
        }
        else if(warningFlag == 0){
              music.stopMusic();
        }
        else{}
    }

    Color GetImgColor(Color color)
    {
        // 透明度
        time += Time.deltaTime * speed * 5.0f;
        if (distanceOfDeath <= dis[0])
        {
            color.a = (Mathf.Sin(time) + 0.2f) / 2.0f;
            if(musicflag == false && warningFlag != 1){
                warningFlag = 1;
                musicflag = true;
            }
            else if(musicflag == true && warningFlag == 1){
                warningFlag = 3;
            }
            else if(musicflag == true && warningFlag == 3){
                warningFlag = 3;
            }

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
            warningFlag = 0;
            musicflag = false;
        }

        color.r = 0.3f; // 赤色
        
        return color;
    }
}
