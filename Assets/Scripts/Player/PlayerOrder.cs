using Constants;
using SharedLibrary;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PlayerOrder : MonoBehaviour
    {
        [SerializeField] private GameObject orderObject;
        [SerializeField] private TextMeshProUGUI orderValue;
        [SerializeField] private TextMeshProUGUI orderSuit;
    
        public void SetValue(int value)
        {
            orderObject.SetActive(true);
            orderValue.text = value.ToString();
        }
    
        public void SetSuit(Suit suit)
        {
            orderSuit.gameObject.SetActive(true);
            orderSuit.text = RankSuitFormatting.GetSuitString(suit);
            orderSuit.color = Colors.GetSuitColor(suit);
        }
    
        public void ClearOrder()
        {
            orderValue.text = "";
            orderSuit.text = "";
            orderSuit.gameObject.SetActive(false);
            orderObject.SetActive(false);
        }
    }
}
