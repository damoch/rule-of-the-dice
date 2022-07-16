using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public int TurnNumber;
    public List<CardData> CardsSource;
    // Start is called before the first frame update
    void Start()
    {
        
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
}
