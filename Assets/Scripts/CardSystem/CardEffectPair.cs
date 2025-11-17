using System;
using UnityEngine.Localization;
using System.Linq;

namespace CardSystem
{
    [Serializable]
    public struct CardEffectPair
    {
        public CardEffectCategory category;
        public EffectArgs args;

        public string Description()
        {
            return GetLocalizedDescription(category, args);
        }

        public static string GetLocalizedDescription(CardEffectCategory category, EffectArgs args)
        {
            var localized = new LocalizedString("RogueEffects", category.ToString());

            string template = localized.GetLocalizedString();

            return string.Format(template, (args.AllValue ?? Array.Empty<string>()).Cast<object>().ToArray());
        }

        public void Action()
        {
            CardEffectRegistry.Action(this);
        }

        public void Action(EffectArgs targetArgs)
        {
            CardEffectRegistry.Action(this, targetArgs);
        }
    }
}