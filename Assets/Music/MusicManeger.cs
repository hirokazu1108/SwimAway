using UnityEngine;

public class MusicManeger : MonoBehaviour
{

    public static MusicManeger  instance;
    public AudioSource audioSource;
void Awake ()
{
 if (instance == null) {
 
     instance = this;
     DontDestroyOnLoad (gameObject);
 }
 else {
 
     Destroy (gameObject);
 }
}

public void stopMusic(){
    Debug.Log("stop");
    audioSource.Stop();     
}

public void startMusic(){
   audioSource.Play(); 
}

}
