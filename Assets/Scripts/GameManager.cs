using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private bool isPauseGame = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            isPauseGame = !isPauseGame;
            if (isPauseGame) PauseGame(); else ResumeGame();
        }
    }

    public static void GameOver()
    {
        Debug.Log("GameOver");

        Retry();
    }

    //�Q�[�����͂��߂���ǂݍ���
    public static void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    //�|�[�Y
    public static void PauseGame()
    {
        Time.timeScale = 0;
    }

    //�ĊJ
    public static void ResumeGame()
    {
        Time.timeScale = 1;
    }
}
