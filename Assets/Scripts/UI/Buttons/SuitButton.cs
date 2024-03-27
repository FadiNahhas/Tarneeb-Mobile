using SharedLibrary;
using TMPro;
using UnityEngine;
using Constants;
using Events;

public class SuitButton : MonoBehaviour
{
    [SerializeField] private Suit suit;
    
    [Header("References")]
    [SerializeField] private TextMeshProUGUI buttonText;

    public void OnClick() => GlobalEvents.InvokeLocalPlayerChoseTrump(suit);

    private void OnValidate()
    {
        UpdateButtonText();
    }
        
    private void UpdateButtonText()
    {
        if (buttonText == null)
        {
            Debug.LogWarning($"Suit button with value {suit} does not have a button text component.");
            return;
        }
        
        switch (suit)
        {
            case Suit.Clubs or Suit.Spades:
                ColorUtility.TryParseHtmlString(Colors.Black, out var color);
                buttonText.color = color;
                break;
            case Suit.Diamonds or Suit.Hearts:
                ColorUtility.TryParseHtmlString(Colors.Red, out color);
                buttonText.color = color;
                break;
        }
        
        buttonText.text = RankSuitFormatting.GetSuitString(suit);
    }
}
