using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnitSystem;

public class HpBar : MonoBehaviour
{
    [Header("Components")]
    public Slider hpSlider;
    public TextMeshProUGUI hpText;

    private Defender defender;

    private void Start()
    {
        // GameController.Instance.onBattleStart += OnBattleStart;
        // GameController.Instance.onWaveStart += OnWaveStart;

        // if (!GameController.Instance.IsFight) OnWaveStart();
    }

    private void OnDestroy()
    {
        // if (GameController.Instance != null) GameController.Instance.onBattleStart -= OnBattleStart;
        // if (GameController.Instance != null) GameController.Instance.onWaveStart -= OnWaveStart;

        if (defender != null)
        {
            defender.onCurrentHpChanged -= OnHpChanged;
            defender.onMaxHpChanged -= OnHpChanged;
        }
    }

    public void SetDefender(Defender defender) => SetDefender(defender, Vector3.zero);

    public void SetDefender(Defender defender, Vector3 offset, bool isFollow = true)
    {
        this.defender = defender;

        if (isFollow)
        {
            FollowUI followUI = gameObject.AddComponent<FollowUI>();
            followUI.SetTarget(defender.transform, offset);
        }

        defender.onCurrentHpChanged += OnHpChanged;
        defender.onMaxHpChanged += OnHpChanged;

        OnHpChanged(defender.CurrentHp);
    }

    public void OnHpChanged(int hp)
    {
        if (defender.MaxHp == 0) return;

        hpSlider.value = (float)defender.CurrentHp / (float)defender.MaxHp;
        hpText.text = $"{defender.CurrentHp} / {defender.MaxHp}";
    }
}
