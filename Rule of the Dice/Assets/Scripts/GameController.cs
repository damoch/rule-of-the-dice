using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int TurnNumber;
    public List<CardData> CardsSource;
    public CardGUI[] CardsOnHandGUI;
    public Dictionary<int,CardData> CardsOnHand;
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
        Debug.Log("Next turn");
        TurnNumber++;
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
        CardsOnHandGUI[id].DescriptionText.text = cardData.Description;
    }
}
