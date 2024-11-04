using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController0 : MonoBehaviour
{
    [SerializeField] PlayerController0 playerController0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        playerController0.HandleUpdate();
    }
}
