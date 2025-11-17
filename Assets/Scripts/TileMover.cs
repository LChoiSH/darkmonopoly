using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileMover : MonoBehaviour
{
    public float moveTile = 0.2f;

    

    public void Move(Vector2 pos)
    {
        transform.position = pos;
    }
}
