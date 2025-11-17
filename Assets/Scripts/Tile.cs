using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private SpriteRenderer renderer;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void Active()
    {

    }

    public void ChangeColor(Color color)
    {
        if (renderer == null) renderer = GetComponent<SpriteRenderer>();

        renderer.color = color;

    }
}
