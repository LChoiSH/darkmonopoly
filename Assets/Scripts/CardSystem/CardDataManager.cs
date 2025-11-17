using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CardSystem;
using Unity.VisualScripting;

public class CardDataManager : MonoBehaviour
{
    public static CardDataManager Instance;

    [Header("Card Collections")]
    [SerializeField] private List<Card> availableCards;

    public IReadOnlyList<Card> AvailableCards => availableCards;

    // public Card Card(string id) => availableCards[card];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    //[ContextMenu("ReadCSV")]
    //private async Task ReadCSV()
    //{
    //    string CSVPath = "Assets/CSV/Cards.csv";

    //    List<Card> cards = new List<Card>();

    //    List<Dictionary<string, object>> value = await ReadCSVTask(CSVPath);

    //    CSVReader.ReadByAddressablePath(CSVPath, (List<Dictionary<string, object>> value) =>
    //    {
    //        foreach (var data in value)
    //        {
    //            string title = data["title"].ToString().Replace(" ", "");
    //            string type = data["type"].ToString(); 
    //            Type cardType = Type.GetType(title);

    //            if (cardType != null && typeof(Card).IsAssignableFrom(cardType))
    //            {
    //                Card cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //                Debug.Log($"Added card: {cardInstance.Title}");

    //                cards.Add(cardInstance);
    //            }
    //            else if (cardType == null && type == "attack")
    //            {
    //                cardType = Type.GetType("AttackCard");
    //                Card cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //                Debug.Log($"Added attack card: {cardInstance.Title}");

    //                cards.Add(cardInstance);
    //            }
    //            else
    //            {
    //                Debug.LogWarning($"Card class '{title}' not found or is not a valid Card type.");
    //            }

    //            //Card newCard = new Card(data);

    //        }

    //        this.availableCards = cards;

    //        Debug.Log("Finish Reading cards");

    //        //EditorUtility.SetDirty(this);
    //        //AssetDatabase.SaveAssets();
    //    });
    //}

    // public async Task ReadCSVAsyncLegacy()
    // {
    //     string CSVPath = "Assets/CSV/Cards.csv";

    //     List<Card> cards = new List<Card>();

    //     List<Dictionary<string, object>> value = await ReadCSVTask(CSVPath);

    //     foreach (var data in value)
    //     {
    //         string title = data["title"].ToString().Replace(" ", "");
    //         string type = data["type"].ToString();
    //         Type cardType = Type.GetType(title);

    //         if (cardType != null && typeof(Card).IsAssignableFrom(cardType))
    //         {
    //             Card cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //             await cardInstance.FindSpriteAsync();
    //             //Debug.Log($"Added card: {cardInstance.Title}");

    //             // if (cardInstance is AttackCard) await ((AttackCard)cardInstance).LoadMissilePrefabAsync();

    //             cards.Add(cardInstance);

    //         }
    //         else if (cardType == null && type == "attack")
    //         {
    //             cardType = Type.GetType("AttackCard");
    //             Card cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //             await cardInstance.FindSpriteAsync();
    //             //Debug.Log($"Added attack card: {cardInstance.Title}");
    //             // await ((AttackCard)cardInstance).LoadMissilePrefabAsync();
    //             cards.Add(cardInstance);
    //         }
    //         else if (cardType == null && type == "support")
    //         {
    //             cardType = Type.GetType("SupportCard");
    //             Card cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //             await cardInstance.FindSpriteAsync();
    //             //Debug.Log($"Added attack card: {cardInstance.Title}");
    //             //await ((AttackCard)cardInstance).LoadMissilePrefabAsync();
    //             cards.Add(cardInstance);
    //         }
    //         {
    //             Debug.LogWarning($"Card class '{title}' not found or is not a valid Card type.");
    //         }

    //     }

    //     this.availableCards = cards;

    //     cardDic = new Dictionary<int, Card>();
    //     foreach (var card in availableCards)
    //     {
    //         cardDic[card.Id] = card;
    //     }

    //     Debug.Log("Finish Reading cards");
    // }

    // [ContextMenu("ReadCSV")]
    // public async Task ReadCSVAsync()
    // {
    //     string CSVPath = "Assets/CSV/Cards.csv";

    //     List<Card> cards = new List<Card>();

    //     List<Dictionary<string, object>> value = await ReadCSVTask(CSVPath);

    //     foreach (var data in value)
    //     {
    //         //string title = data["title"].ToString().Replace(" ", "");
    //         string type = data["type"].ToString();
    //         Type cardType;
    //         Card cardInstance;

    //         if (type == "attack" || type == "install")
    //         {
    //             cardType = Type.GetType("AttackCard");
    //             cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //             // await ((AttackCard)cardInstance).LoadMissilePrefabAsync();
    //         }
    //         else
    //         {
    //             cardType = Type.GetType("SupportCard");
    //             cardInstance = (Card)Activator.CreateInstance(cardType, data);
    //         }

    //         cards.Add(cardInstance);
    //         await cardInstance.FindSpriteAsync();
    //     }

    //     this.availableCards = cards;

    //     cardDic = new Dictionary<int, Card>();
    //     foreach (var card in availableCards)
    //     {
    //         cardDic[card.Id] = card;
    //     }

    //     Debug.Log("Finish Reading cards");
    // }

    public Card GetRandomAvailableCard()
    {
        if (AvailableCards != null && AvailableCards.Count > 0)
        {
            int randomIndex = UnityEngine.Random.Range(0, AvailableCards.Count);
            return AvailableCards[randomIndex];
        }
        else
        {
            Debug.LogWarning("No available cards to select from.");
            return null;
        }
    }

    public List<Card> GetRandomCards(int count = 3)
    {
        if (availableCards == null || availableCards.Count == 0)
        {
            Debug.LogWarning("카드 리스트가 비어있습니다.");
            return null;
        }

        if (availableCards.Count < count)
        {
            count = availableCards.Count;
        }


        if (availableCards.Count <= count)
        {
            return new List<Card>(availableCards); // 카드가 적으면 가능한 모든 카드 반환
        }

        List<Card> randomCards = new List<Card>(availableCards);
        System.Random random = new System.Random();

        // 리스트를 랜덤으로 섞음 (Fisher-Yates Shuffle 알고리즘 사용)
        for (int i = randomCards.Count - 1; i > 0; i--)
        {
            int j = random.Next(0, i + 1);
            (randomCards[i], randomCards[j]) = (randomCards[j], randomCards[i]); // Swap
        }

        return randomCards.GetRange(0, count); // 앞에서 3장 선택
    }
}
