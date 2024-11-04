using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 4.0f; // 每个对话框的显示时间
    private float timerDisplay;
    public GameObject dialogBox1;
    public GameObject dialogBox2;
    private Queue<GameObject> dialogBoxes; // 用于存储对话框的队列
    private bool isDialogPlaying; // 检查是否正在播放对话框序列

    void Start()
    {
        // 初始化对话框队列
        dialogBoxes = new Queue<GameObject>();
        dialogBoxes.Enqueue(dialogBox1);
        dialogBoxes.Enqueue(dialogBox2);

        // 初始时不显示任何对话框
        dialogBox1.SetActive(false);
        dialogBox2.SetActive(false);

        isDialogPlaying = false;
    }

    void Update()
    {
        // 如果正在播放对话框并且计时器结束
        if (isDialogPlaying)
        {
            if (timerDisplay > 0)
            {
                timerDisplay -= Time.deltaTime;
            }
            else
            {
                // 当前对话框计时结束，隐藏并播放下一个
                if (dialogBoxes.Count > 0)
                {
                    dialogBoxes.Peek().SetActive(false); // 隐藏当前对话框
                    timerDisplay = displayTime; // 重置计时器
                    dialogBoxes.Dequeue(); // 移除已播放的对话框
                    if (dialogBoxes.Count > 0)
                    {
                        dialogBoxes.Peek().SetActive(true); // 显示下一个对话框
                    }
                }
                else
                {
                    isDialogPlaying = false; // 所有对话框播放完毕
                }
            }
        }
    }

    public void DisplayDialog()
    {
        if (!isDialogPlaying && dialogBoxes.Count > 0)
        {
            isDialogPlaying = true;
            dialogBoxes.Peek().SetActive(true); // 显示第一个对话框
            timerDisplay = displayTime; // 设置计时器
        }
        else if(dialogBoxes.Count <= 0) 
        {
            SceneManager.LoadScene(7);
        }
    }
}