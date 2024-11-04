using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move
{
    public MoveBase Base { get; set; }
    
    public int PP { get; set; }

    public Move(MoveBase pBase)
    {
        if (pBase == null)
        {
            Debug.LogError("MoveBase is null in Move constructor");
        }
        Base = pBase;
        PP = 0;
    }
}
