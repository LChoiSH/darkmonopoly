using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnitSystem
{
    [RequireComponent(typeof(Unit))]
    public class Defender : MonoBehaviour
    {
        private Unit unit;
        private int currentHp;
        private List<IHitModifier> defenseModifiers = new();
        private List<IHealModifier> healModifiers = new();

        public int MaxHp => unit != null ? unit.Stat.maxHp : 0;
        public int CurrentHp => currentHp;
        public float HpRatio => MaxHp > 0 ? currentHp / (float)MaxHp : 0;
        public List<IHitModifier> DefenseModifiers => defenseModifiers;
        public List<IHealModifier> HealModifiers => healModifiers;

        public event Action<int> onCurrentHpChanged;
        public event Action<int> onMaxHpChanged;
        public event Action onDeath;

        private void Awake()
        {
            unit = GetComponent<Unit>();
        }

        private void Start()
        {
            ResetHp();
        }

        public void ResetHp()
        {
            currentHp = MaxHp;
            onMaxHpChanged?.Invoke(MaxHp);
            onCurrentHpChanged?.Invoke(currentHp);
        }

        public void Damaged(Hit hit)
        {
            // Apply defense modifiers
            foreach (var modifier in defenseModifiers)
            {
                hit = modifier.Apply(hit);
            }

            if (hit.finalDamage <= 0) return;

            currentHp = Mathf.Max(0, currentHp - hit.finalDamage);
            onCurrentHpChanged?.Invoke(currentHp);

            // Execute post-hit callbacks
            if (hit.postCallbacks != null)
            {
                foreach (var callback in hit.postCallbacks)
                {
                    callback?.Invoke();
                }
            }

            if (currentHp <= 0)
            {
                onDeath?.Invoke();
            }
        }

        public void Healed(Heal heal)
        {
            // Apply heal modifiers
            foreach (var modifier in healModifiers)
            {
                heal = modifier.Apply(heal);
            }

            if (heal.finalAmount <= 0) return;

            currentHp = Mathf.Min(MaxHp, currentHp + heal.finalAmount);
            onCurrentHpChanged?.Invoke(currentHp);

            // Execute post-heal callbacks
            if (heal.postCallbacks != null)
            {
                foreach (var callback in heal.postCallbacks)
                {
                    callback?.Invoke();
                }
            }
        }

        public void AddDefenseModifier(IHitModifier modifier)
        {
            defenseModifiers.Add(modifier);
        }

        public void RemoveDefenseModifier(IHitModifier modifier)
        {
            defenseModifiers.Remove(modifier);
        }

        public void ClearDefenseModifiers()
        {
            defenseModifiers.Clear();
        }

        public void AddHealModifier(IHealModifier modifier)
        {
            healModifiers.Add(modifier);
        }

        public void RemoveHealModifier(IHealModifier modifier)
        {
            healModifiers.Remove(modifier);
        }

        public void ClearHealModifiers()
        {
            healModifiers.Clear();
        }

        // --------- Editor Debug ---------
        [ContextMenu("Debug HP Info")]
        private void DebugHPInfo()
        {
            Debug.Log($"=== [{gameObject.name}] HP Debug ===\n" +
                      $"Current HP: {currentHp}\n" +
                      $"Max HP: {MaxHp}\n" +
                      $"HP Ratio: {HpRatio:P0}\n" +
                      $"Defense Modifiers: {defenseModifiers.Count}\n" +
                      $"Heal Modifiers: {healModifiers.Count}");
        }
    }
}