using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (StageManager.ExistsNextStage())
            {
                StageManager.GoNextStage();
            }
        }
    }
}
