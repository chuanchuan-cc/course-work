using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class playerInteractionPanel : MonoBehaviour
{
    public GameObject panel;
    public CanvasGroup canvasGroup; 
    public TextMeshProUGUI title;
    public TextMeshProUGUI group;
    public TextMeshProUGUI price;
    public TextMeshProUGUI rent;
    public Button yesButton;
    public Button noButton;
    private System.Action<bool> callback; 
    public bool isResult = false;

    void Start()
    {
        if (canvasGroup == null)
        {
            canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>(); 
            }
        }

        ClosePanel();
    }

    void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void ShowPanel(string message, System.Action<bool> callback)
{
    ShowPanel(message, null, null, null, callback);
}

 public void ShowPanel(string message,string _group, int? _price,int? _rent,System.Action<bool> callback)
    {
        StopAllCoroutines();

        panel.SetActive(true);
        title.text = message;
        this.callback = callback;
        if (_group == null) group.gameObject.SetActive(false);
    else
    {
        group.gameObject.SetActive(true);
        group.text="group: "+_group;
    }

    if (_price == null) price.gameObject.SetActive(false);
    else
    {
        price.gameObject.SetActive(true);
        price.text="price: "+_price.Value.ToString();
    }

    if (_rent == null) {
        rent.text = "";
        rent.gameObject.SetActive(false);
        }
    else
    {
        rent.gameObject.SetActive(true);
        rent.text="rent: "+_rent.Value.ToString();
    }
        
        
        
        yesButton.onClick.RemoveAllListeners();
        noButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => { SetResult(true); });
        noButton.onClick.AddListener(() => { SetResult(false); });
        StartCoroutine(PanelDisplay());
        isResult = false;
    }
    private IEnumerator PanelDisplay()
    {
        isResult = false;
        yield return StartCoroutine(FadeIn());
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut());
        panel.SetActive(false);
    }

    void SetResult(bool result)
    {
       
        isResult = true;
        callback?.Invoke(result);
    }

    private IEnumerator FadeIn()
    {
     

        float duration = 0.25f;
        float elapsedTime = 0f;

        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        while (elapsedTime < duration)
        {  
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        canvasGroup.alpha = 1;
       
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.25f;
        float elapsedTime = 0f;

        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        while (elapsedTime < duration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0;
      
    }
}
