using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TypeWriter : MonoBehaviour
{
    [Header("���ּ��ʱ��")]
    public float speedTime = 0.1f;//���ּ��ʱ��

    float timer;//��ʱ��ʱ��
    TMP_Text TextCompnt;//Text�������
    int wordNumber;//��������
    bool isStart;//�Ƿ�ʼ����

    string wordContent;//----��������

    void Awake()
    {
        TextCompnt = GetComponent<TMP_Text>();//�ӵ�ǰ�����ȡ��Text���
        if( TextCompnt == null )
        {
            Debug.Log("error");
        }
        isStart = true;//boolֵ��Ĭ��ֵ��false  ��������Ҫ����Ϊtrue
        wordContent = "��ã���ӭ����δ������!    \n\n������㽫��������ʿ���ӽ��������磬��ò�һ���ĸ���\n\n" +
            "������ʿ�������̵Ƶ�����ͨ����· \n\n����һ�ؿ��У�����ͨ�������жϹ���·��ʱ��\n\n" +
            "����������һ�ؿ�.....";
    }

    void FixedUpdate()
    {
        StartTyping();
    }
    void StartTyping()
    {
        if (isStart)
        {
            timer += Time.deltaTime;//�򵥵ļ�ʱ��
            if (timer >= speedTime)//�����ʱ��ʱ��>���ּ��ʱ��
            {
                timer = 0;//����
                wordNumber++;//��������+1

                //���ַ�����ָ�����ַ�λ�ÿ�ʼ�Ҿ���ָ���ĳ��ȡ�
                TextCompnt.text = wordContent.Substring(0, wordNumber);//������������ʼ�ַ�λ�ã����㿪ʼ��
                if (wordNumber >= wordContent.Length)//��������=���ֵĳ���
                {
                    isStart = false;//ֹͣ����
                    SceneManager.LoadScene(4);
                }
            }
        }
    }
}

