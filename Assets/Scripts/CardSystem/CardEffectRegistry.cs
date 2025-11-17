using System;
using System.Collections.Generic;
using UnitSystem;
using UnityEngine;

namespace CardSystem
{
    public static class CardEffectRegistry
    {
        public static void Action(CardEffectPair effectPair) => effectMap[effectPair.category].Invoke(effectPair.args);
        public static void Action(CardEffectPair effectPair, EffectArgs targetArgs) => effectMapWithTarget[effectPair.category].Invoke(effectPair.args, targetArgs);

        public static Dictionary<CardEffectCategory, Action<EffectArgs>> effectMap = new Dictionary<CardEffectCategory, Action<EffectArgs>>()
        {
            { CardEffectCategory.Move, Move }
        };

        public static Dictionary<CardEffectCategory, Action<EffectArgs, EffectArgs>> effectMapWithTarget = new Dictionary<CardEffectCategory, Action<EffectArgs, EffectArgs>>()
        {
            { CardEffectCategory.Attack, Attack }
        };

        public static void Move(EffectArgs args)
        {
            GameManager.Instance.moveSystem.MoveSteps(args.Int(0));
        }

        public static void Attack(EffectArgs args, EffectArgs targetArgs)
        {
            int damage = args.Int(0);

            foreach(string target in targetArgs.AllValue)
            {
                Unit targetUnit = UnitManager.Instance.GetUnitByRuntimeId(target);

                if (targetUnit == null)
                {
                    Debug.LogWarning($"Target unit with ID {target} not found");
                    continue;
                }

                if (targetUnit.Defender == null)
                {
                    Debug.LogWarning($"Target unit {target} has no Defender component");
                    continue;
                }

                // Hit 시스템 사용 (attacker는 null, 카드 공격이므로)
                Hit hit = Hit.Create(null, targetUnit.Defender, damage);
                targetUnit.Defender.Damaged(hit);
            }
        }
    }
}