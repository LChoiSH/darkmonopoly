using System.Collections.Generic;
using UnityEngine;

namespace UnitSystem
{
    [RequireComponent(typeof(Unit))]
    public class Attacker : MonoBehaviour
    {
        private Unit unit;
        private List<IHitModifier> attackModifiers = new();

        public int Damage => unit != null ? unit.Stat.damage : 0;
        public List<IHitModifier> AttackModifiers => attackModifiers;

        private void Awake()
        {
            unit = GetComponent<Unit>();
        }

        public void Attack(Defender defender)
        {
            if (unit == null || defender == null) return;

            Hit hit = Hit.Create(this, defender, Damage);

            // Apply all attack modifiers
            foreach (var modifier in attackModifiers)
            {
                hit = modifier.Apply(hit);
            }

            defender.Damaged(hit);
        }

        public void AddModifier(IHitModifier modifier)
        {
            attackModifiers.Add(modifier);
        }

        public void RemoveModifier(IHitModifier modifier)
        {
            attackModifiers.Remove(modifier);
        }

        public void ClearModifiers()
        {
            attackModifiers.Clear();
        }
    }
}