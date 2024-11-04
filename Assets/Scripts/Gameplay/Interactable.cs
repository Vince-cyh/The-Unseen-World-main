using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Interactable
{
    void Interact(Collider2D collider);
    void Cheater(Collider2D collider);
}
