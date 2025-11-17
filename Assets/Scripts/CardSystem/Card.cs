using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace CardSystem
{
    [Serializable]
    public class Card
    {
        public int id;
        public string title;
        public string description;
        public Sprite sprite;
        public int baseCost;            // Inspector에 노출되는 기본 코스트
        private int costModifier = 0;   // 코스트 증감값
        public CardType cardType;
        public bool needsTarget;
        public List<CardEffectPair> pairs;

        public int Id => id;
        public string Title => title;
        public string Description => DescriptionText();
        public Sprite Sprite => sprite;
        public int BaseCost => baseCost;
        public int Cost => Mathf.Max(0, baseCost + costModifier);  // UI/사용 시 최소 0
        public int CostModifier
        {
            get => costModifier;
            set
            {
                if (costModifier != value)
                {
                    costModifier = value;
                    onCostChanged?.Invoke(Cost);
                }
            }
        }
        public CardType CardType => cardType;
        public bool NeedsTarget => needsTarget;

        private Card originCard;

        public Action<Card> onPlayed;
        public Action<int> onCostChanged;

        public static char[] split = { ',' };
        public static char[] actionSplit = { ' ' };

#if UNITY_EDITOR
        // public Card(Dictionary<string, object> value)
        // {
        //     this.id = int.Parse(value["id"].ToString());
        //     this.title = value["title"].ToString();
        //     this.description = value["description"].ToString();
        //     this.cost = int.Parse(value["cost"].ToString());

        //     string typeString = value["type"].ToString();
        //     Enum.TryParse(Char.ToUpper(typeString[0]) + typeString.Substring(1), out cardType);

        //     // after play
        //     string[] afterPlayStrings = new string[0];
        //     if (value["after_play"].ToString() != "")
        //     {
        //         afterPlayStrings = value["after_play"].ToString().Split(split).Select(f => f.Trim()).ToArray();
        //     }

        //     afterPlayAction = new List<CardAction>();

        //     for (int i = 0; i < afterPlayStrings.Length; i++)
        //     {
        //         string[] actionSplits = afterPlayStrings[i].Split(actionSplit);
        //         Type actionType;

        //         if (actionSplits == null || actionSplits.Length < 1)
        //         {
        //             actionType = Type.GetType("CardActions.Discard");
        //         }
        //         else
        //         {
        //             actionType = Type.GetType("CardActions." + actionSplits[i]);
        //         }

        //         CardAction actionInstane = (CardAction)Activator.CreateInstance(actionType);

        //         if (actionSplits.Length > 1)
        //         {
        //             actionInstane.Set(actionSplits);
        //         }

        //         afterPlayAction.Add(actionInstane);
        //     }

        //     if (afterPlayAction.Count <= 0)
        //     {
        //         Type actionType;
        //         actionType = Type.GetType("CardActions.Discard");
        //         CardAction actionInstane = (CardAction)Activator.CreateInstance(actionType);
        //         afterPlayAction.Add(actionInstane);
        //     }
        // }
#endif

        public Card(int id, string title, string description, Sprite sprite, int cost, CardType cardType, bool needsTarget = false)
        {
            this.id = id;
            this.title = title;
            this.description = description;
            this.sprite = sprite;
            this.baseCost = cost;
            this.cardType = cardType;
            this.needsTarget = needsTarget;
        }

        public Card(Card other)
        {
            originCard = other;

            this.id = other.id;
            this.title = other.title;
            this.description = other.description;
            this.sprite = other.sprite; // Sprite�� ���� (�ʿ��ϸ� ���� ����)
            this.baseCost = other.baseCost;
            this.costModifier = other.costModifier;
            this.cardType = other.cardType;
            this.needsTarget = other.needsTarget;

            pairs = new();

            if (other.pairs != null)
            {
                for (int i = 0; i < other.pairs.Count; i++)
                {
                    var originalPair = other.pairs[i];
                    var clonedPair = new CardEffectPair
                    {
                        category = originalPair.category,
                        args = originalPair.args.Clone()
                    };
                    pairs.Add(clonedPair);
                }
            }
        }

        public Card()
        {

        }

        public virtual Card Clone()
        {
            return new Card(this); // �Ϲ� Card��� �״�� ����
        }

        public static async Task<Card> CreateAsyncCard(Dictionary<string, object> value)
        {
            Card madeCard = new Card();

            madeCard.id = int.Parse(value["id"].ToString());
            madeCard.title = value["title"].ToString();
            madeCard.description = value["description"].ToString();
            madeCard.baseCost = int.Parse(value["cost"].ToString());
            //this.range = int.Parse(value["range"].ToString());

            //FindSprite();
            await madeCard.FindSpriteAsync();

            return madeCard;
        }

        public void DisplayInfo()
        {
            Debug.Log($"Name: {title}, Description: {description}, Cost: {Cost}");
        }

        public async Task FindSpriteAsync()
        {
            string assetKey = $"Assets/Image/Cards/{title.Replace(" ", "")}.png";

            // bool keyExists = Addressables.ResourceLocators
            //     .SelectMany(locator => locator.Keys)
            //     .Contains(assetKey);

            // if (keyExists)
            // {
            //     AsyncOperationHandle<Sprite> handle = Addressables.LoadAssetAsync<Sprite>(assetKey);
            //     await handle.Task;

            //     if (handle.Status == AsyncOperationStatus.Succeeded)
            //     {
            //         sprite = handle.Result;
            //     }
            //     else
            //     {
            //         sprite = null;
            //     }
            // }
        }

#if UNITY_EDITOR
        private void FindSprite()
        {
            string folderPath = "Assets/Image/Cards";

            string[] guids = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath });
            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                if (fileName.Equals(this.title.Replace(" ", ""), System.StringComparison.OrdinalIgnoreCase))
                {
                    sprite = AssetDatabase.LoadAssetAtPath<Sprite>(assetPath);
                }
            }
        }
#endif

        public virtual void Play(EffectArgs? targetArgs = null)
        {
            // Debug.Log($"Card Play: {title}");

            for(int i = 0; i < pairs.Count; i++)
            {
                // 타겟이 제공되고, 해당 effect가 타겟을 필요로 하면 타겟과 함께 실행
                if (targetArgs.HasValue && pairs[i].category == CardEffectCategory.Attack)
                {
                    pairs[i].Action(targetArgs.Value);
                }
                else
                {
                    pairs[i].Action();
                }
            }

            onPlayed?.Invoke(this);
        }

        public void DrawPlay()
        {
        }

        public void AfterPlayAction()
        {
        }

        public virtual string DescriptionText()
        {
            string returnValue = "";

            return returnValue;
        }
    }
}