using System.Collections.Generic;
using UnitSystem;
using Unity.VisualScripting;
using UnityEngine;
using VInspector;

public class Enemy : MonoBehaviour
{
    public int attackCount = 3;
    public int damage = 1; 

    private List<int> attackTiles;

    void Start()
    {
        if (GameManager.Instance.turnSystem.state != TurnSystem.TurnState.Prepare)
        {
            PrepareAttack();
        }

        GameManager.Instance.turnSystem.onPrepare += PrepareAttack;
        GameManager.Instance.turnSystem.onEnemyTurn += Attack;
    }

    [Button]
    private void PrepareAttack()
    {
        attackTiles = GameManager.Instance.map.GetRandomTiles(attackCount);

        foreach(int tileNum in attackTiles)
        {
            Tile tile = GameManager.Instance.map.Node(tileNum);

            tile.ChangeColor(Color.red);
        }
    }
    
    [Button]
    private void Attack()
    {
        int unitIndex = GameManager.Instance.moveSystem.CurrentIndex;

        if(attackTiles.Contains(unitIndex))
        {
            GameManager.Instance.player.Defender.Damaged(Hit.Create(null, GameManager.Instance.player.Defender, damage));
        }

        foreach(int tileNum in attackTiles)
        {
            Tile tile = GameManager.Instance.map.Node(tileNum);

            tile.ChangeColor(Color.white);
        }
    }
}