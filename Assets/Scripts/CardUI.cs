using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardUI : MonoBehaviour
{
    public GameObject cardPanel;   
    public TextMeshProUGUI cardDescription;
    public TextMeshProUGUI cardTitle;  
    private Card currentCard;   
    public bool isDisplaying=false;
    void Start()
    {
        cardPanel.SetActive(false); 
    }

    public void ShowCard(Card card)
    {
        currentCard = card;
        cardDescription.text = card.description.Replace("?", "").Replace("\"", "");  
        cardTitle.text=card.group;
        cardPanel.SetActive(true); 
        isDisplaying=true;
    }
    

    public void HideCard()
    {
        cardPanel.SetActive(false); 
        isDisplaying=false;
    }
}
