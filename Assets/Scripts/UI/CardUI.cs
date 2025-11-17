using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardSystem;

public class CardUI : MonoBehaviour
{
    // TODO: change to drag
    public Button actionButton;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI manaCostText;

    private Card setCard;

    public Card Card => setCard;

    void OnDestroy()
    {
        if (setCard != null)
        {
            setCard.onCostChanged -= OnCostChanged;
        }
    }

    public void SetCard(Card card)
    {
        // 이전 카드 이벤트 구독 해제
        if (setCard != null)
        {
            setCard.onCostChanged -= OnCostChanged;
        }

        this.setCard = card;

        titleText.text = card.title;
        descriptionText.text = card.description;
        UpdateCostText(card.Cost);

        // 새 카드 이벤트 구독
        card.onCostChanged += OnCostChanged;

        actionButton.onClick.RemoveListener(UseCard);
        actionButton.onClick.AddListener(UseCard);
    }

    private void OnCostChanged(int newCost)
    {
        UpdateCostText(newCost);
    }

    private void UpdateCostText(int cost)
    {
        if (manaCostText != null)
        {
            manaCostText.text = cost.ToString();
        }
    }
    
    public void UseCard()
    {
        if (setCard == null)
        {
            Debug.LogError($"Card dont have setcard");
            return;
        }

        CardController.Instance.UseCard(setCard);
    }
}
