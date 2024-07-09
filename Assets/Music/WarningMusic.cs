using UnityEngine;

public class WarningMusic : MonoBehaviour
{

    public static WarningMusic  instance;
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
    audioSource.Stop();     
}

public void startMusic(){
    Debug.Log("narasu");
   audioSource.Play(); 
}

}