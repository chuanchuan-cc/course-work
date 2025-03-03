using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardUI : MonoBehaviour
{
    public GameObject cardPanel;   // 卡片 UI 面板
    public TextMeshProUGUI cardText;  // 卡片文本
    public Button closeButton;     // 关闭按钮
    private Card currentCard;       // 记录当前显示的卡片

    void Start()
    {
        cardPanel.SetActive(false);  // 初始隐藏卡片
        closeButton.onClick.AddListener(HideCard);
    }

    public void ShowCard(Card card)
    {
        currentCard = card;
        cardText.text = card.description;  // 显示卡片描述
        cardPanel.SetActive(true);  // 显示卡片 UI
    }

    public void HideCard()
    {
        cardPanel.SetActive(false);  // 隐藏 UI
    }
}
