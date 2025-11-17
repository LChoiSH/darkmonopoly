
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace UnitSystem
{
    public struct Hit
    {
        public Attacker attacker;
        public Defender defender;
        public int baseDamage;
        public int finalDamage;

        public List<Action> postCallbacks;

        public static Hit Create(Attacker attacker, Defender defender, int baseDamage)
        {
            return new Hit
            {
                attacker = attacker,
                defender = defender,
                baseDamage = baseDamage,
                finalDamage = baseDamage,
                postCallbacks = new List<Action>()
            };
        }
    }
}