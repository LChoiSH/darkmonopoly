using System.Collections;
using System.Collections.Generic;
using CardSystem;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnitSystem;

public class CardCanvas : MonoBehaviour
{
    [Header("UI Components")]
    public CardUI cardItem;
    public RectTransform cardWrap;
    public TextMeshProUGUI drawCountText;
    public TextMeshProUGUI discardCountText;

    [Header("Drag Controller")]
    public HandCardDragController dragController;

    public Dictionary<Card, CardUI> madeCardDic = new();

    void Start()
    {
        Debug.Log($"Canvas add: {CardController.Instance.HandCards.Count} cards");

        if (CardController.Instance.HandCards != null)
        {
            foreach (Card card in CardController.Instance.HandCards)
            {
                OnHandAdded(card);
            }
        }

        CardController.Instance.onHandAdded += OnHandAdded;
        CardController.Instance.onHandDiscard += OnHandDiscard;

        OnDiscardChanged(CardController.Instance.DiscardPile);
        CardController.Instance.onDiscardChanged += OnDiscardChanged;

        OnDrawChanged(CardController.Instance.DrawPile);
        CardController.Instance.onDrawChanged += OnDrawChanged;

        // 드래그 이벤트 구독
        if (dragController != null)
        {
            dragController.OnCardUsed += OnCardDropped;
        }
    }

    void OnDestroy()
    {
        if (CardController.Instance != null)
        {
            CardController.Instance.onHandAdded -= OnHandAdded;
            CardController.Instance.onHandDiscard -= OnHandDiscard;
            CardController.Instance.onDiscardChanged -= OnDiscardChanged;
            CardController.Instance.onDrawChanged -= OnDrawChanged;
        }

        if (dragController != null)
        {
            dragController.OnCardUsed -= OnCardDropped;
        }
    }

    private void OnHandAdded(Card card)
    {
        Debug.Log($"{card.title} added");

        CardUI madeCard = Instantiate(cardItem, cardWrap);
        madeCard.SetCard(card);

        madeCardDic[card] = madeCard;
    }

    private void OnHandDiscard(Card card)
    {
        Destroy(madeCardDic[card].gameObject);
        madeCardDic.Remove(card);
    }

    private void OnDiscardChanged(List<Card> cards)
    {
        discardCountText.text = $"Discard: {cards.Count}";
    }

    private void OnDrawChanged(List<Card> cards)
    {
        drawCountText.text = $"Draw: {cards.Count}";
    }

    private void OnCardDropped(CardUI cardUI, Vector3 hitPoint)
    {
        if (cardUI == null)
        {
            Debug.LogWarning("CardUI is null");
            return;
        }

        Card card = cardUI.Card;
        if (card == null)
        {
            Debug.LogWarning("CardUI has no Card");
            return;
        }

        Debug.Log($"Card '{card.title}' dropped at {hitPoint}");

        // 타겟이 필요한 카드인지 확인
        if (card.NeedsTarget)
        {
            // 타겟 찾기
            Unit targetUnit = FindUnitAtPosition(hitPoint);

            if (targetUnit == null)
            {
                Debug.LogWarning($"Card '{card.title}' needs target, but no unit found at {hitPoint}");
                return; // 카드 사용 취소 (손으로 복귀)
            }

            // targetArgs 생성 (유닛 런타임 ID 전달)
            EffectArgs targetArgs = EffectArgs.From(targetUnit.GetInstanceID().ToString());

            // 마나 확인 후 타겟과 함께 카드 사용
            GameManager.Instance.manaSystem.UseMana(card.Cost, () =>
            {
                card.Play(targetArgs);
                CardController.Instance.DiscardCard(card);
            });
        }
        else
        {
            // 타겟 불필요 (Move 등)
            CardController.Instance.UseCard(card);
        }
    }

    private Unit FindUnitAtPosition(Vector3 worldPos)
    {
        // 해당 위치에 있는 유닛 찾기 (Physics2D 사용)
        Collider2D hit = Physics2D.OverlapPoint(worldPos);

        if (hit != null)
        {
            Unit unit = hit.GetComponent<Unit>();
            if (unit != null)
            {
                return unit;
            }
        }

        return null;
    }

    // TODO: Tile 타겟 카드를 위한 메서드 (나중에 구현)
    // private Tile FindTileAtPosition(Vector3 worldPos) { }
}
