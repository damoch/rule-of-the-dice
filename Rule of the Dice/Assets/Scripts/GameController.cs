using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int TurnNumber;
    public List<CardData> CardsSource;
    public CardGUI[] CardsOnHandGUI;
    public Dictionary<int,CardData> CardsOnHand;
    public Queue<CardData> CardsQueue;
    // Start is called before the first frame update
    void Start()
    {
        CardsOnHand = new Dictionary<int, CardData>
        {
            { 0, GetNewCard() },
            { 1, GetNewCard() },
            { 2, GetNewCard() },
            { 3, GetNewCard() }
        };

        foreach (var item in CardsOnHand)
        {
            SetCardGuiFor(item.Key, item.Value);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextTurn()
    {
        TurnNumber++;
        Debug.Log("Turn " + TurnNumber);
    }

    private CardData GetNewCard()
    {
        var result = Random.Range(0, CardsSource.Count - 1);
        var card = CardsSource[result];
        Debug.Log("Card generated: " + card.Description);
        return card;
    }

    private void SetCardGuiFor(int id, CardData cardData)
    {
        ClearCardGui(CardsOnHandGUI[id]);

        CardsOnHandGUI[id].DescriptionText.text = cardData.Description;
        CardsOnHandGUI[id].DurationInTurnsText.text = $"Duration: {cardData.DurationInTurns} turns";

        if(cardData.HappinesValue > 0)
        {
            CardsOnHandGUI[id].ProsText.text += "Happines++\n";
        }
        if (cardData.HappinesValue < 0)
        {
            CardsOnHandGUI[id].ConsText.text += "Happines--\n";
        }

        if (cardData.PollutionValue > 0)
        {
            CardsOnHandGUI[id].ConsText.text += "Pollution++\n";
        }
        if (cardData.PollutionValue < 0)
        {
            CardsOnHandGUI[id].ProsText.text += "Pollution--\n";
        }

        if (cardData.MoneyValue > 0)
        {
            CardsOnHandGUI[id].ProsText.text += "Money++\n";
        }
        if (cardData.MoneyValue < 0)
        {
            CardsOnHandGUI[id].ConsText.text += "Money--\n";
        }
        CardsOnHandGUI[id].DiscardCostText.text = $"Discard: {cardData.DiscardCost} credits";
    }

    private void ClearCardGui(CardGUI cardGUI)
    {
        cardGUI.ConsText.text = string.Empty;
        cardGUI.ProsText.text = string.Empty;
        cardGUI.DescriptionText.text = string.Empty;
        cardGUI.DurationInTurnsText.text = string.Empty;
        cardGUI.DiscardCostText.text = string.Empty;
    }

    public void QueueCard(int id)
    {
        var card = CardsOnHand[id];
        Debug.Log($"Queueing card: {card.Description}");
        var newCard = GetNewCard();
        CardsOnHand[id] = newCard;
        SetCardGuiFor(id, newCard);
    }
}
