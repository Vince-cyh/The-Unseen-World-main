using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public void StartOver()
    {
        SceneManager.LoadScene(2);
    }

    public void GoMenu()
    {
        SceneManager.LoadScene(0);
    }
}
