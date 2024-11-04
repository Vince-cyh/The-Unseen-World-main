using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 4.0f; // ÿ���Ի������ʾʱ��
    private float timerDisplay;
    public GameObject dialogBox1;
    public GameObject dialogBox2;
    private Queue<GameObject> dialogBoxes; // ���ڴ洢�Ի���Ķ���
    private bool isDialogPlaying; // ����Ƿ����ڲ��ŶԻ�������

    void Start()
    {
        // ��ʼ���Ի������
        dialogBoxes = new Queue<GameObject>();
        dialogBoxes.Enqueue(dialogBox1);
        dialogBoxes.Enqueue(dialogBox2);

        // ��ʼʱ����ʾ�κζԻ���
        dialogBox1.SetActive(false);
        dialogBox2.SetActive(false);

        isDialogPlaying = false;
    }

    void Update()
    {
        // ������ڲ��ŶԻ����Ҽ�ʱ������
        if (isDialogPlaying)
        {
            if (timerDisplay > 0)
            {
                timerDisplay -= Time.deltaTime;
            }
            else
            {
                // ��ǰ�Ի����ʱ���������ز�������һ��
                if (dialogBoxes.Count > 0)
                {
                    dialogBoxes.Peek().SetActive(false); // ���ص�ǰ�Ի���
                    timerDisplay = displayTime; // ���ü�ʱ��
                    dialogBoxes.Dequeue(); // �Ƴ��Ѳ��ŵĶԻ���
                    if (dialogBoxes.Count > 0)
                    {
                        dialogBoxes.Peek().SetActive(true); // ��ʾ��һ���Ի���
                    }
                }
                else
                {
                    isDialogPlaying = false; // ���жԻ��򲥷����
                }
            }
        }
    }

    public void DisplayDialog()
    {
        if (!isDialogPlaying && dialogBoxes.Count > 0)
        {
            isDialogPlaying = true;
            dialogBoxes.Peek().SetActive(true); // ��ʾ��һ���Ի���
            timerDisplay = displayTime; // ���ü�ʱ��
        }
        else if(dialogBoxes.Count <= 0) 
        {
            SceneManager.LoadScene(7);
        }
    }
}