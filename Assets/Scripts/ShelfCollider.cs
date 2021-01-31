using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShelfCollider : BoxCollider
{
    void Awake()
    {
        this.tag = "Shelf";
    }   
}
