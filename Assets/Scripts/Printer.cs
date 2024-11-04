using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TypeWriter : MonoBehaviour
{
    [Header("打字间隔时间")]
    public float speedTime = 0.1f;//打字间隔时间

    float timer;//计时器时间
    TMP_Text TextCompnt;//Text文字组件
    int wordNumber;//文字数量
    bool isStart;//是否开始打字

    string wordContent;//----文字内容

    void Awake()
    {
        TextCompnt = GetComponent<TMP_Text>();//从当前物体获取到Text组件
        if( TextCompnt == null )
        {
            Debug.Log("error");
        }
        isStart = true;//bool值的默认值是false  所以这里要重置为true
        wordContent = "你好，欢迎来到未见世界!    \n\n在这里，你将以视障人士的视角体验世界，获得不一样的感受\n\n" +
            "视障人士聆听红绿灯的声音通过马路 \n\n在下一关卡中，请你通过听觉判断过马路的时机\n\n" +
            "正引导至下一关卡.....";
    }

    void FixedUpdate()
    {
        StartTyping();
    }
    void StartTyping()
    {
        if (isStart)
        {
            timer += Time.deltaTime;//简单的计时器
            if (timer >= speedTime)//如果计时器时间>打字间隔时间
            {
                timer = 0;//重置
                wordNumber++;//文字数量+1

                //子字符串从指定的字符位置开始且具有指定的长度。
                TextCompnt.text = wordContent.Substring(0, wordNumber);//数字数量的起始字符位置（从零开始）
                if (wordNumber >= wordContent.Length)//数字数量=文字的长度
                {
                    isStart = false;//停止打字
                    SceneManager.LoadScene(4);
                }
            }
        }
    }
}

