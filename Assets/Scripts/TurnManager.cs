using System;
using System.Collections.Generic;
using UnityEditor.Animations;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private MapBuilder mapBuilder;
    public TileMover mainUnit;
    private int mainUnitIndex;

    private List<Tile> tiles;

    void Start()
    {
        tiles = mapBuilder.madeTiles;
    }

    public void MoveUnit(int n)
    {
        
    }

}
