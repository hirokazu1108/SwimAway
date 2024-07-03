using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleUIManager : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            if (StageManager.ExistsNextStage())
            {
                StageManager.GoNextStage();
            }
        }
    }
}
