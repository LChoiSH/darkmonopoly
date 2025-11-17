using System;
using UnityEngine;

[Serializable]
public class ManaSystem
{
    [SerializeField] private int _startMana;
    [SerializeField] private int _maxMana;

    private int _mana;
    private int _currentMana;


    public int Mana => _mana;
    public int CurrentMana => _currentMana;

    public event Action<int> OnMpChanged;
    // public event Action<int> onManaFill;

    public void Init()
    {
        SetMana(_startMana);
    }

    public void SetMana(int nextMana)
    {
        if (nextMana > _maxMana) nextMana = _maxMana;

        _mana = nextMana;
    }

    public void UseMana(int useAmount, Action callBack = null)
    {
        if (_currentMana < useAmount) return;

        _currentMana -= useAmount;

        OnMpChanged?.Invoke(_currentMana);
        callBack?.Invoke();
    }

    public void ManaFill(int fillAmount)
    {
        _currentMana += fillAmount;
        _currentMana = Mathf.Min(_currentMana, _mana);

        OnMpChanged?.Invoke(_currentMana);
    }
}