using System;
using System.Collections.Generic;
using UnityEngine;
using VInspector;
using System.Linq;
using System.Threading.Tasks;
using CardSystem;
using UnityEngine.XR;

public class CardController : MonoBehaviour
{
    public static CardController Instance { get; private set; }

    private List<Card> deck;
    private List<Card> drawPile;
    private HashSet<Card> handCards;
    private List<Card> discardPile;

    [SerializeField] private List<int> startCards;

    private Dictionary<int, Card> cardDic;

    public List<Card> Deck => deck;
    public List<Card> DrawPile => drawPile;
    public IReadOnlyList<Card> HandCards => handCards != null ? handCards.ToList() : null;
    public List<Card> DiscardPile => discardPile;
    public Card GetCard(int id) => cardDic[id]; 
    public bool IsDrawEmpty => drawPile.Count == 0;

    public Action<Card> onHandDiscard;
    public Action<Card> onHandAdded;
    public Action<Card> onCardRemoved;
    public Action<List<Card>> onDiscardChanged;
    public Action<List<Card>> onDrawChanged;

    private void Awake()
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

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if(CardDataManager.Instance == null)
        {
            Debug.LogError("CardDataManager doesnt exist!");
            return;
        }

        drawPile = new List<Card>();
        handCards = new();
        discardPile = new List<Card>();
        deck = new List<Card>();

        cardDic = new();

        var availableCards = CardDataManager.Instance.AvailableCards;

        foreach (Card card in availableCards)
        {
            cardDic[card.id] = card;
        }

        if (startCards != null)
        {
            foreach (int id in startCards)
            {
                if (cardDic.ContainsKey(id))
                {
                    AddToDeck(cardDic[id]);
                }
            }
        }

        Debug.Log("Card Controller Initialized");
    }

    public void ResetDrawPileFromDeck()
    {
        DrawPile.Clear();
        ResetHandCards();
        ResetDiscardCards();
        DrawPile.AddRange(Deck);

        Debug.Log("Card Draw pile Reset");
    }

    public void UseCard(Card card, bool useCost = true)
    {
        if (!handCards.Contains(card))
        {
            Debug.LogWarning($"{card.title} doesnt exist in hand");
            return;
        }

        if (useCost)
        {
            GameManager.Instance.manaSystem.UseMana(card.Cost, () =>
            {
                card.Play();
                DiscardCard(card);
            });
        }
        
    }

    public void ResetHandCards()
    {
    }

    public void ResetDiscardCards()
    {
        discardPile = new List<Card>();
    }

    public void ShuffleDrawPile()
    {
        for (int i = 0; i < DrawPile.Count; i++)
        {
            Card temp = DrawPile[i];
            int randomIndex = UnityEngine.Random.Range(0, DrawPile.Count);
            DrawPile[i] = DrawPile[randomIndex];
            DrawPile[randomIndex] = temp;
        }

        Debug.Log("DrawPile shuffled.");
    }
    
    public void AddHandCard(Card card)
    {
        handCards.Add(card);
        onHandAdded?.Invoke(card);
    }

    public void DrawCards(int count)
    {
        for(int i = 0;i < count;i++)
        {
            Card card = DrawCard();
            if (card == null) break;
        }
    }

    public Card DrawCard()
    {
        if (IsDrawEmpty) DiscardToDraw();

        if (DrawPile.Count > 0)
        {
            Card drawnCard = DrawPile[0];
            drawPile.RemoveAt(0);
            onDrawChanged?.Invoke(drawPile);

            AddHandCard(drawnCard);

            return drawnCard;
        }
        else
        {
            Debug.Log("Draw pile is empty!");
            return null;
        }
    }

    public void DiscardCard(Card card)
    {
        if (!handCards.Contains(card))
        {
            Debug.LogError($"Card remove fail: {card.title} doesnt exist.");
            return;
        }

        handCards.Remove(card);
        AddDiscard(card);
        onHandDiscard?.Invoke(card);
    }

    public void DiscardAllCard()
    {
        while (handCards.Count > 0)
        {
            var enumerator = handCards.GetEnumerator();
            enumerator.MoveNext();
            DiscardCard(enumerator.Current);
        }
    }

    public void AddDiscard(Card card)
    {
        DiscardPile.Add(card);
        onDiscardChanged?.Invoke(discardPile);
    }

    private void DiscardToDraw()
    {
        if(discardPile.Count <= 0)
        {
            Debug.Log("discard pile is empty");
            return;
        }

        drawPile.AddRange(discardPile);

        discardPile.Clear();
        onDiscardChanged?.Invoke(discardPile);

        ShuffleDrawPile();

        onDrawChanged?.Invoke(drawPile);
    }

    public void AddToDeck(Card card)
    {
        deck.Add(card.Clone());
    }

    /// <summary>
    /// Debug용 메서드: 각 파일의 카드 개수 출력 (에디터 전용)
    /// </summary>
    [ContextMenu("Debug Card Counts")]
    public void DebugCardCounts()
    {
#if UNITY_EDITOR
        string debugMessage = $"[Card Counts Debug]\n" +
            $"Deck: {deck?.Count ?? 0} cards\n" +
            $"DrawPile: {drawPile?.Count ?? 0} cards\n" +
            $"HandCards: {handCards?.Count ?? 0} cards\n" +
            $"DiscardPile: {discardPile?.Count ?? 0} cards\n" +
            $"---\n" +
            $"Total: {(deck?.Count ?? 0) + (drawPile?.Count ?? 0) + (handCards?.Count ?? 0) + (discardPile?.Count ?? 0)} cards";
        
        Debug.Log(debugMessage);
#endif
    }
}