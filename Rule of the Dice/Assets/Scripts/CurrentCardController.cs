using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CurrentCardController : MonoBehaviour
{
    public Text CardDesc;
    public Text HappinessText;
    public Text PollutionText;
    public Text MoneyText;
    public Text LongDescriptionText;
    public Text DurationText;

    public void SetCardData(CardData cardData)
    {
        PollutionText.enabled = cardData.PollutionValue != 0;
        HappinessText.enabled = cardData.HappinesValue != 0;
        MoneyText.enabled = cardData.MoneyValue != 0;
        CardDesc.text = cardData.Description;

        HappinessText.text = $"Population happiness: {cardData.HappinesValue}/turn";
        PollutionText.text = $"Pollution: {cardData.PollutionValue}/turn";
        MoneyText.text = $"Credits: {cardData.MoneyValue}/turn";
        LongDescriptionText.text = cardData.LongDescription;
        DurationText.text = $"Law stays in power for {cardData.DurationInTurns} turns";

        HappinessText.color = cardData.HappinesValue > 0 ? Color.green : Color.red;
        PollutionText.color = cardData.PollutionValue < 0 ? Color.green : Color.red;
        MoneyText.color = cardData.MoneyValue > 0 ? Color.green : Color.red;
    }
}
