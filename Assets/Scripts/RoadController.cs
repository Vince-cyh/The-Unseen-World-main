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
    
    private float timer = 0f; // ���ڼ�ʱ�ı���
    private float checkInterval = 7f; // ���ü����Ϊ7��
    private bool isAllowedToCross = false; // ��ʼ״̬����ͨ����·

    // Start is called before the first frame update
    void Start()
    {
        if (player != null)
        {
            float playerY = player.transform.position.y;
            Debug.Log("��ҵ�y������: " + playerY);
        }
        successDialogue .SetActive(false);
        failDialogue .SetActive(false);
        slowAudio = slowAudioObject.GetComponent<AudioSource>();
        fastAudio = fastAudioObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        // ���¼�ʱ��
        timer += Time.deltaTime;

        // ����Ƿ�ﵽ7��ı���
        if (timer >= checkInterval)
        {
            // �л�����ͨ����·��״̬
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
            // ���ü�ʱ��
            timer = 0f;
        }

        // ��ȡ��ҵ�y����
        float playerY = player.transform.position.y;

        // �������Ƿ�����·��y����������
        if (playerY >= 0 && playerY <= 17)
        {
            // ���ݵ�ǰ״̬�����Ƿ���Ҫ���¼��س���
            if (!isAllowedToCross)
            {
                ReloadCurrentSceneAfterFail();
            }
        }

        // �����ҵ�y�������20�����۵�ǰ״̬��Σ�����ת����һ������
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
            // �ȴ�����
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
        // ���¼��ص�ǰ����
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
         
    }

    void GoToNextScene()
    {
        // ��ת����һ������
        SceneManager.LoadScene(2);
    }
}