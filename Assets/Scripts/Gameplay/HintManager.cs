using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour
{
    public GameObject player;
    public float displayDistance = 5.0f;
    public Canvas Hint;
    private bool isDialogActive = false;

    void Update()
    {
        if (!isDialogActive)
        {
            float distance = Vector3.Distance(player.transform.position, transform.position);
            Hint.gameObject.SetActive(distance <= displayDistance);
        }
    }

    public void SetDialogActive(bool isActive)
    {
        isDialogActive = isActive;
        Hint.gameObject.SetActive(!isActive);
    }
}
