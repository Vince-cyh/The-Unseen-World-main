using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject tutorialCanvas;

    // �򿪽̳̻���
    public void ShowTutorial()
    {
        tutorialCanvas.SetActive(true);
    }

    // �رս̳̻���
    public void HideTutorial()
    {
        tutorialCanvas.SetActive(false);
    }
}
