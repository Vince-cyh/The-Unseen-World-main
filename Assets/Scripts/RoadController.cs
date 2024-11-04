using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoadController : MonoBehaviour
{
    public GameObject player;
    public GameObject successDialogue;
    public GameObject failDialogue;
    public GameObject slowAudioObject;
    public GameObject fastAudioObject;
    AudioSource slowAudio;
    AudioSource fastAudio;
    
    private float timer = 0f; // 用于计时的变量
    private float checkInterval = 7f; // 设置检查间隔为7秒
    private bool isAllowedToCross = false; // 初始状态允许通过马路

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            float playerY = player.transform.position.y;
            Debug.Log("玩家的y坐标是: " + playerY);
        }
        successDialogue .SetActive(false);
        failDialogue .SetActive(false);
        slowAudio = slowAudioObject.GetComponent<AudioSource>();
        fastAudio = fastAudioObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // 更新计时器
        timer += Time.deltaTime;

        // 检查是否达到7秒的倍数
        if (timer >= checkInterval)
        {
            // 切换允许通过马路的状态
            isAllowedToCross = !isAllowedToCross;
            Debug.Log("7 seconds passed, changing state to " + (isAllowedToCross ? "allowed" : "not allowed"));
            if(!isAllowedToCross)
            {
                slowAudio.Play();
                fastAudio.Stop();
            }
            else { 
                fastAudio.Play(); 
                slowAudio.Stop();
            }
            // 重置计时器
            timer = 0f;
        }

        // 获取玩家的y坐标
        float playerY = player.transform.position.y;

        // 检查玩家是否在马路的y坐标区间内
        if (playerY >= 0 && playerY <= 17)
        {
            // 根据当前状态决定是否需要重新加载场景
            if (!isAllowedToCross)
            {
                ReloadCurrentSceneAfterFail();
            }
        }

        // 如果玩家的y坐标大于20，无论当前状态如何，都跳转到下一个场景
        if (playerY > 17)
        {
            ShowSuccessAndGoNext();
        }
    }

    void ShowSuccessAndGoNext()
    {
        if (successDialogue != null)
        {
            successDialogue.SetActive(true);
            // 等待两秒
            Invoke("GoToNextScene", 2f);
        }
    }

    void ReloadCurrentSceneAfterFail()
    {
        failDialogue.SetActive(true);
        Invoke("ReloadCurrentScene", 2f);
    }

    void ReloadCurrentScene()
    {
        // 重新加载当前场景
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
         
    }

    void GoToNextScene()
    {
        // 跳转到下一个场景
        SceneManager.LoadScene(2);
    }
}