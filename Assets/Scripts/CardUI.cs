using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class CardUI : MonoBehaviour
{
    public GameObject cardPanel;   // 卡片 UI 面板
    public TextMeshProUGUI cardText;  // 卡片文本 
    private Card currentCard;      // 记录当前显示的卡片
    public bool isDisplaying=false;
    void Start()
    {
        cardPanel.SetActive(false);  // 初始隐藏卡片
    }

    public void ShowCard(Card card)
    {
        currentCard = card;
        cardText.text = card.description.Replace("?", "").Replace("\"", "");  // 显示卡片描述
        cardPanel.SetActive(true);  // 显示卡片 UI
        isDisplaying=true;
    }
    

    public void HideCard()
    {
        cardPanel.SetActive(false);  // 隐藏 UI
        isDisplaying=false;
    }
}
