using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] string questionMarkName; // 问号对象的名称
    [SerializeField] bool isBattle;

    public event Action<Collider2D> OnEncountered;

    public void Interact(Collider2D collider)
    {
        if(!isBattle)
        {
            StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
            foreach (Transform child in transform)
            {
                if (child.name == questionMarkName)
                {
                    Destroy(child.gameObject);
                    break; // 找到并销毁后跳出循环
                }
            }
        }
        else
        {
            StartCoroutine(HandleEncounter(collider));
        }
    }

    private IEnumerator HandleEncounter(Collider2D collider)
    {
        // 启动 ShowDialog 协程并等待其结束
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

        // 在 ShowDialog 结束后执行后续操作
        PlayerController.animator.SetBool("isMoving", false);
        OnEncountered(collider);

        yield return new WaitForEndOfFrame();

        Destroy(collider.gameObject);
    }

    public void Cheater(Collider2D collider)
    {
        StartCoroutine(HandleCheater(collider));
    }

    private IEnumerator HandleCheater(Collider2D collider)
    {
        // 启动 ShowDialog 协程并等待其结束
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

        SceneManager.LoadScene(5);

        yield return new WaitForEndOfFrame();

        Destroy(collider.gameObject);
    }
}