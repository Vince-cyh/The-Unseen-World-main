using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    [SerializeField] string questionMarkName; // �ʺŶ��������
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
                    break; // �ҵ������ٺ�����ѭ��
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
        // ���� ShowDialog Э�̲��ȴ������
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

        // �� ShowDialog ������ִ�к�������
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
        // ���� ShowDialog Э�̲��ȴ������
        yield return StartCoroutine(DialogManager.Instance.ShowDialog(dialog));

        SceneManager.LoadScene(5);

        yield return new WaitForEndOfFrame();

        Destroy(collider.gameObject);
    }
}