using Assets.Scripts;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text TurnNumberText;
    public Text UpcomingCardsList;
    public int TurnNumber;
    public List<CardData> CardsSource;
    public CardGUI[] CardsOnHandGUI;
    public Dictionary<int,CardData> CardsOnHand;
    public Queue<CardData> CardsQueue;
    public CurrentCardController CurrentCardController;
    public int MinimalNumberOfCardsInQueue;
    public Button NextTurnButton;
    private Dictionary<DiceController, bool> _hasDiceEndedRoll;
    public int CurrentDicerollValue;
    // Start is called before the first frame update
    void Start()
    {
        NextTurnButton.interactable = false;
        CardsQueue = new Queue<CardData>();
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
        //NextTurn();
    }

    internal void RegisterDice(DiceController diceController)
    {
        if(_hasDiceEndedRoll == null)
        {
            _hasDiceEndedRoll = new Dictionary<DiceController, bool>();
        }
        Debug.Log("Registered dice " + diceController.gameObject.name);
        _hasDiceEndedRoll.Add(diceController, true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NextTurn()
    {
        NextTurnButton.interactable = false;
        TurnNumber++;
        Debug.Log("Turn " + TurnNumber);
        TurnNumberText.text = $"Turn: {TurnNumber}";
        StartDiceRoll();
    }

    internal void NotifyDicerollResult(int value, DiceController dc)
    {
        _hasDiceEndedRoll[dc] = true;

        foreach (var dice in _hasDiceEndedRoll.Keys)
        {
            if (!_hasDiceEndedRoll[dice])
            {
                return;
            }
        }

        EndDiceRoll();
    }

    private void EndDiceRoll()
    {
        var result = _hasDiceEndedRoll.Keys.Sum(x => x.Value);

        Debug.Log($"Result = {result}");
    }

    private void StartDiceRoll()
    {
        //Ugly-ass hack :/
        var d1 = _hasDiceEndedRoll.Keys.First();
        var d2 = _hasDiceEndedRoll.Keys.Last();
        _hasDiceEndedRoll[d1] = false;
        _hasDiceEndedRoll[d2] = false;
        d1.BeginDiceRoll();
        d2.BeginDiceRoll();

        //foreach (var dice in _hasDiceEndedRoll.Keys)
        //{
        //    dice.BeginDiceRoll();
        //    _hasDiceEndedRoll[dice] = false;
        //}
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
        CardsQueue.Enqueue(card);
        Debug.Log($"Queueing card: {card.Description}");
        var newCard = GetNewCard();
        CardsOnHand[id] = newCard;
        SetCardGuiFor(id, newCard);
        SetCardQueue();
        NextTurnButton.interactable = CardsQueue.Count >= MinimalNumberOfCardsInQueue;
    }

    private void SetCardQueue()
    {
        var cardArray = CardsQueue.ToArray();
        CurrentCardController.SetCardData(cardArray[0]);
        UpcomingCardsList.text = string.Empty;
        if (cardArray.Length > 0)
        {
            for (int i = 1; i < cardArray.Length; i++)
            {
                UpcomingCardsList.text += $"{cardArray[i].Description}\n";
            }
        }
    }
}
