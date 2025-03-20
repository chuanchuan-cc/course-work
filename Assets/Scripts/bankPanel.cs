using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class bankPanel : MonoBehaviour
{
    public GameObject choosePanel;
    public GameObject behaviourPanel;
    public CanvasGroup bankCanvasGroup; 
    public CanvasGroup interactionCanvasGroup;
    public Button sellHouseButton;
    public Button sellPropertyButton;
    public Button mortgageButton;
    public Button remdeemButton;
    public Button quitButton;
    private System.Action<bool> callback; 
    public bool isResult = false;
    public GameObject estate;
  
    

    void Start()
    {
       if (bankCanvasGroup == null){
            bankCanvasGroup = choosePanel.GetComponent<CanvasGroup>();
       
            if (bankCanvasGroup == null)
            {
                bankCanvasGroup = choosePanel.AddComponent<CanvasGroup>(); 
            }
       
       }

        ClosePanel();
    }

    public void ClosePanel()
    {
        choosePanel.SetActive(false);
    }


 public void ShowPanel()
    {
       
        choosePanel.SetActive(true);


        StartCoroutine(FadeIn());
        sellHouseButton.onClick.AddListener(sellHouse);
        sellPropertyButton.onClick.AddListener(sellProperty);
        mortgageButton.onClick.AddListener(mortgage);
        remdeemButton.onClick.AddListener(remdeem);
        quitButton.onClick.AddListener(quit);
        
    }
    public void ShowBehaviourPanel(string message,Player player,System.Action<bool> callback)
    {

    }
    private void sellHouse(){
      StartCoroutine(FadeOut());
    }
    private void sellProperty(){
StartCoroutine(FadeOut());
    }
    private void mortgage(){
StartCoroutine(FadeOut());
    }
    private void remdeem(){
       StartCoroutine(FadeOut()); 
    }
    private void quit(){
        StartCoroutine(FadeOut());
    }
    private IEnumerator PanelDisplay()
    {
        isResult = false;
        yield return StartCoroutine(FadeIn());
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut());
        behaviourPanel.SetActive(false);
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

        bankCanvasGroup.interactable = true;
        bankCanvasGroup.blocksRaycasts = true;

        while (elapsedTime < duration)
        {  
            bankCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        bankCanvasGroup.alpha = 1;
       
    }

    private IEnumerator FadeOut()
    {
        float duration = 0.25f;
        float elapsedTime = 0f;

        bankCanvasGroup.interactable = false;
        bankCanvasGroup.blocksRaycasts = false;

        while (elapsedTime < duration)
        {
            bankCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        bankCanvasGroup.alpha = 0;
      
    }
}

