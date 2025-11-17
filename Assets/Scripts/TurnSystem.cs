using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.PlayerLoop;

public sealed class TurnSystem
{
    public enum TurnState { Prepare, Player, Enemy, End }

    public TurnState state = TurnState.Prepare;

    public event Action onPrepare;
    public event Action onPlayerStart;
    public event Action onPlayerEnd;
    public event Action onEnemyTurn;
    public event Action onEndTurn;

    public void Init()
    {
        this.state = TurnState.Prepare;
        PrePareTurn();
    }

    private void PrePareTurn()
    {
        onPrepare?.Invoke();

        ChangeState(TurnState.Player);
    }

    public void PlayerEnd()
    {
        if (state != TurnState.Player) return;

        onPlayerEnd?.Invoke();

        ChangeState(TurnState.Enemy);
    }

    private void EnemyTurn()
    {
        onEnemyTurn?.Invoke();

        ChangeState(TurnState.End);
    }

    private void EndTurn()
    {
        onEndTurn?.Invoke();

        ChangeState(TurnState.Prepare);
    }
    
    private void ChangeState(TurnState state)
    {
        if (this.state == state) return;

        this.state = state;
        Debug.Log($"turn: {state}");

        switch(state)
        {
            case TurnState.Prepare:
                PrePareTurn();
                break;
            case TurnState.Player:
                onPlayerStart?.Invoke();
                break;
            case TurnState.Enemy:
                EnemyTurn();
                break;
            case TurnState.End:
                EndTurn();
                break;
        }
    }

}
