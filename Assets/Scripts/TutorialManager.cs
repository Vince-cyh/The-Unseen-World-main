using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;

    // 打开教程画布
    public void ShowTutorial()
    {
        tutorialCanvas.SetActive(true);
    }

    // 关闭教程画布
    public void HideTutorial()
    {
        tutorialCanvas.SetActive(false);
    }
}
