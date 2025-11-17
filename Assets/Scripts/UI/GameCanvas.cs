using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameCanvas : MonoBehaviour
{
    public Button moveButton;
    public Button turnEndButton;

    public TextMeshProUGUI hpText;
    public TextMeshProUGUI mpText;

    void Start()
    {
        moveButton.onClick.AddListener(Move);
        turnEndButton.onClick.AddListener(() => GameManager.Instance.turnSystem.PlayerEnd());

        // Debug.Log($"hp: {GameManager.Instance.unit.CurrentHp}");
        OnHpChanged(GameManager.Instance.player.Defender.CurrentHp);
        GameManager.Instance.player.Defender.onCurrentHpChanged += OnHpChanged;
        
        OnMpChanged(GameManager.Instance.manaSystem.CurrentMana);
        GameManager.Instance.manaSystem.OnMpChanged += OnMpChanged;

        GameManager.Instance.turnSystem.onPlayerStart += OnPlayerTurn;
        GameManager.Instance.turnSystem.onPlayerEnd += OnPlayerTurnEnd;
    }

    private void Move()
    {
        // int random = Random.Range();

        GameManager.Instance.manaSystem.UseMana(1, () => GameManager.Instance.moveSystem.MoveSteps(1));
    }

    private void OnHpChanged(int hp)
    {
        hpText.text = hp.ToString();
    }

    private void OnMpChanged(int mp)
    {
        mpText.text = mp.ToString();
    }

    private void OnPlayerTurn()
    {
        moveButton.gameObject.SetActive(true);
        turnEndButton.gameObject.SetActive(true);
    }
    
    private void OnPlayerTurnEnd()
    {
        moveButton.gameObject.SetActive(false);
        turnEndButton.gameObject.SetActive(false);
    }
}
