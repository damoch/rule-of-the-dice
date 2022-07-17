using Assets.Scripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public Text ResultText;
    public Text ResultDescText;
    public Text WarningsText;
    public Text TurnNumberText;
    public Text UpcomingCardsList;
    public Text ActiveCardsList;
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
    public CountryStats Stats;
    public Text StatsText;
    public List<CardData> AppliedPolicies;
    public Rules Rules;
    public int CurrentTurnsInDebt;
    public int CurrentTurnsInExceedingPollution;
    public int TurnsOfUnhappiness;
    // Start is called before the first frame update
    void Start()
    {
        CurrentCardController.gameObject.SetActive(false);
        UpcomingCardsList.gameObject.transform.parent.gameObject.SetActive(false);
        ActiveCardsList.gameObject.transform.parent.gameObject.SetActive(false);
        AppliedPolicies = new List<CardData>();
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
        UpdateStats();
        TurnNumberText.text = $"Turn: {TurnNumber}";
        ResultDescText.enabled = false;
        ResultText.enabled = false;
    }

    private void UpdateStats()
    {
        var creditsPerTurn = AppliedPolicies.Sum(x => x.MoneyValue);
        var happinessPerTurn = AppliedPolicies.Sum(x => x.HappinesValue);
        var pollutionPerTurn = AppliedPolicies.Sum(x => x.PollutionValue);
        var sb = new StringBuilder();
        sb.Append($"Population Happiness: {Stats.Happiness}");
        if(happinessPerTurn != 0)
        {
            sb.Append($"({happinessPerTurn}/turn)");
        }
        sb.Append(Environment.NewLine);

        sb.Append($"Pollution level: {Stats.Pollution}");
        if (pollutionPerTurn != 0)
        {
            sb.Append($"({pollutionPerTurn}/turn)");
        }
        sb.Append(Environment.NewLine);

        sb.Append($"Credits: {Stats.Money}");

        if (creditsPerTurn != 0)
        {
            sb.Append($"({creditsPerTurn}/turn)");
        }
        sb.Append(Environment.NewLine);

        StatsText.text = sb.ToString();
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
        var policy = CardsQueue.Dequeue();
        if (result > 5)
        {
            AppliedPolicies.Add(policy);
            ResultDescText.enabled = true;
            ResultDescText.text = $"{policy.Description} passed!";
            ResultText.color = Color.green;
            ResultDescText.color = Color.green;
        }
        else
        {
            ResultDescText.enabled = true;
            ResultDescText.text = $"{policy.Description} rejected";
            CardsQueue.Dequeue();
            ResultText.color = Color.red;
            ResultDescText.color = Color.red;
        }
        ResultText.enabled = true;
        ResultText.text = $"Result: {result}";
        SetCardQueue();
        ApplyPolicies();
        CheckLosingConditions();
        NextTurnButton.interactable = CardsQueue.Count >= MinimalNumberOfCardsInQueue;
    }

    private void ApplyPolicies()
    {
        if(AppliedPolicies.Count == 0)
        {
            return;
        }
        var newList = new List<CardData>();
        foreach (var item in AppliedPolicies)
        {
            Stats.Happiness += item.HappinesValue;
            Stats.Pollution += item.PollutionValue;
            Stats.Money += item.MoneyValue;

            if(item.DurationInTurns - 1 <= 0)
            {
                continue;
            }
            var ymp = item;
            ymp.DurationInTurns--;
            newList.Add(ymp);
        }
        AppliedPolicies = newList;
        UpdateActivePolicyList();
        UpdateStats();
        ValidateDiscardButtons();
    }

    private void UpdateActivePolicyList()
    {
        ActiveCardsList.text = string.Empty;
        if(AppliedPolicies.Count == 0)
        {
            return;
        }
        foreach (var item in AppliedPolicies)
        {
            ActiveCardsList.text += $"{item.Description} - {item.DurationInTurns} turns\n";
        }

        ActiveCardsList.gameObject.transform.parent.gameObject.SetActive(true);
    }

    private void StartDiceRoll()
    {
        ResultDescText.enabled = false;
        ResultText.enabled = false;
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
        var result = UnityEngine.Random.Range(0, CardsSource.Count - 1);
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

    public void DiscardCard(int id)
    {
        Debug.Log($"Discarding {id}");
        if (Stats.Money < CardsOnHand[id].DiscardCost)
        {
            return;
        }
        Debug.Log($"Discarding {id}");
        Stats.Money -= CardsOnHand[id].DiscardCost;
        var newCard = GetNewCard();
        CardsOnHand[id] = newCard;
        SetCardGuiFor(id, newCard);
        UpdateStats();
        ValidateDiscardButtons();
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
        if (!CurrentCardController.gameObject.activeInHierarchy)
        {
            CurrentCardController.gameObject.SetActive(true);
        }
        CurrentCardController.SetCardData(cardArray[0]);
        UpcomingCardsList.text = string.Empty;
        if (cardArray.Length > 0)
        {
            UpcomingCardsList.gameObject.transform.parent.gameObject.SetActive(cardArray.Length > 1);
            for (int i = 1; i < cardArray.Length; i++)
            {
                UpcomingCardsList.text += $"{cardArray[i].Description}\n";
            }
        }
    }

    private void ValidateDiscardButtons()
    {
        for (int i = 0; i < CardsOnHandGUI.Length; i++)
        {
            CardGUI item = CardsOnHandGUI[i];
            item.DiscardButton.interactable = CardsOnHand[i].DiscardCost <= Stats.Money;
        }
    }

    private void CheckLosingConditions()
    {
        WarningsText.text = string.Empty;


        if(Stats.Money < 0)
        {
            CurrentTurnsInDebt++;
            if(CurrentTurnsInDebt >= Rules.MaxTurnsInDebt)
            {
                GameOver();
            }
            WarningsText.text += $"Turns until bankrupcy: {Rules.MaxTurnsInDebt - CurrentTurnsInDebt}\n";
        }
        else
        {
            CurrentTurnsInDebt = 0;
        }

        if(Stats.Pollution >= Rules.PollutionTreshold)
        {
            CurrentTurnsInExceedingPollution++;
            if (CurrentTurnsInExceedingPollution >= Rules.MaxTurnsInMaxPollution)
            {
                GameOver();
            }
            WarningsText.text += $"Turns until environmental disaster: {Rules.MaxTurnsInMaxPollution - CurrentTurnsInExceedingPollution}\n";
        }
        else 
        {
            CurrentTurnsInExceedingPollution = 0;
        }

        if (Stats.Happiness <= Rules.UnhappinessTreshold)
        {
            TurnsOfUnhappiness++;
            if (TurnsOfUnhappiness >= Rules.MaxTurnsOfMaxUnhappiness)
            {
                GameOver();
            }
            WarningsText.text += $"Turns until revolution: {Rules.MaxTurnsOfMaxUnhappiness - TurnsOfUnhappiness}\n";
        }
        else
        {
            TurnsOfUnhappiness = 0;
        }
    }

    private void GameOver()
    {
        Debug.Log("Game over");
        Debug.Break();
    }
}
