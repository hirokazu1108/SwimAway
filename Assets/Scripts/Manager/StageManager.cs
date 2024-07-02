using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    private static List<string> _stageNameList = new List<string>()
    {
        "GameScene",
        "GameScene2",
    };
    private static int _nextStageIndex = 0;

    public static int StageNum()
    {
        return _stageNameList.Count;
    }

    public static int CurrentStageIndex()
    {
        return _nextStageIndex - 1;
    }

    public static void GoTitle()
    {
        _nextStageIndex = 0;
        SceneManager.LoadScene("TitleScene");
    }

    public static bool ExistsNextStage()
    {
        return _nextStageIndex < _stageNameList.Count;
    }

    public static void GoNextStage()
    {
        if (!ExistsNextStage()) return;

        var nextStage = _stageNameList[_nextStageIndex++];
        SceneManager.LoadScene(nextStage);
    }

    public static bool IsFirstStage()
    {
        return _nextStageIndex == 1;
    }

}
