using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class bankPanel : MonoBehaviour
{
    public GameObject choosePanel;
    public GameObject behaviourPanel;
    public CanvasGroup bankCanvasGroup; 
    public CanvasGroup interactionCanvasGroup;

    public Button sellButton;
    public Button mortgageButton;
    public Button remdeemButton;
    public Button quitButton;
    private System.Action<bool> callback; 
    public bool isResult = false;
    public GameObject estatePrefab;
    public Button behaviourQuit;
    public Button confirmButton;
  public TextMeshProUGUI message;
  private Player player;
  public GameObject generateZone;
  public bool isbanking=false;
  public GameBehaviour gameBehaviour;
  private List<string> operationList= new List<string>();
  private List<Board> mapList;
  public GameObject bankruptPanel;
  public Button brSellButton;
  public Button brmortgageButton;
  public CanvasGroup brCanvasGroup;
    

    void Start()
    {
       if (bankCanvasGroup == null){
            bankCanvasGroup = choosePanel.GetComponent<CanvasGroup>();
       
            if (bankCanvasGroup == null)
            {
                bankCanvasGroup = choosePanel.AddComponent<CanvasGroup>(); 
            }
       
       }
       if (interactionCanvasGroup == null){
            interactionCanvasGroup = behaviourPanel.GetComponent<CanvasGroup>();
       
            if (interactionCanvasGroup == null)
            {
                interactionCanvasGroup = behaviourPanel.AddComponent<CanvasGroup>(); 
            }
            
       
       }
         if (brCanvasGroup == null){
            brCanvasGroup = bankruptPanel.GetComponent<CanvasGroup>();
       
            if (brCanvasGroup == null)
            {
                brCanvasGroup = bankruptPanel.AddComponent<CanvasGroup>(); 
            }
            
       
       }
       mapList=RunGame.mapList;
       gameBehaviour = GameObject.Find("BehaviourPool").GetComponent<GameBehaviour>();



        ClosePanel();
    }
    public void setmap(List<Board> i){
        mapList=i;

    }

    public void ClosePanel()
    {
        choosePanel.SetActive(false);
        behaviourPanel.SetActive(false);
        bankruptPanel.SetActive(false);
        isbanking=false;
    }
    public void setPlayer(Player _player){
        player=_player;
    }


public void showbankruptPanel(){
    bankruptPanel.SetActive(true);
    StartCoroutine(FadeIn(brCanvasGroup));
    brmortgageButton.onClick.AddListener(mortgage);
    brSellButton.onClick.AddListener(sell);

}

 public void ShowPanel()
    {
       
       
        choosePanel.SetActive(true);
        


        StartCoroutine(FadeIn(bankCanvasGroup));

        sellButton.onClick.AddListener(sell);
        mortgageButton.onClick.AddListener(mortgage);
        remdeemButton.onClick.AddListener(remdeem);
        quitButton.onClick.AddListener(quit);
        
    }
    public void ShowBehaviourPanel(string _message)
    {
        operationList.Clear();
      

        behaviourPanel.SetActive(true);
        message.text=_message;
        


        StartCoroutine(FadeIn(interactionCanvasGroup));
        behaviourQuit.onClick.AddListener(quitBehaviour);
          while (generateZone.transform.childCount > 0)
    {
        GameObject.DestroyImmediate(generateZone.transform.GetChild(0).gameObject);
    }
        

    }
    private void sell(){
        

        ShowBehaviourPanel("which estate you are willing to sell");
        generateAssets(false);
        generateAssets(true);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(()=>confirm("makeSell"));
      
    }
    private void mortgage(){
        ShowBehaviourPanel("which estate you are willing to mortage");
        generateAssets(false);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(()=>confirm("makeMortgage"));
    } 
    private void remdeem(){
        ShowBehaviourPanel("which estate you are willing to remdeem");
        generateAssets(true);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(()=>confirm("makeRemdeem"));
    }
    private void quit(){
        StartCoroutine(FadeOut(bankCanvasGroup));
    }
    private IEnumerator PanelDisplay()
    {
        isResult = false;
        yield return StartCoroutine(FadeIn(bankCanvasGroup));
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut(bankCanvasGroup));
        choosePanel.SetActive(false);
    }

    void SetResult(bool result)
    {
       
        isResult = true;
        callback?.Invoke(result);
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        isbanking=true;
     

        float duration = 0.25f;
        float elapsedTime = 0f;

        cg.interactable = true;
        cg.blocksRaycasts = true;

        while (elapsedTime < duration)
        {  
            cg.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        cg.alpha = 1;
       
    }
private void generateAssets(bool i){
      

        
            foreach(Board _board in player.playerData.assetsList){
                estateBoard eBoard = _board as estateBoard;{
                    if(eBoard==null){
                        
                BuyableBoard bBoard=_board as BuyableBoard;
                if(bBoard.isMortgage==i){
                
                GameObject o = GameObject.Instantiate(estatePrefab, generateZone.transform);
                o.name = bBoard.property;
                
                TextMeshProUGUI property = o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI price = o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI rent = o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI group= o.transform.Find("group").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI mortgageState= o.transform.Find("isMortgage").GetComponent<TextMeshProUGUI>();

                property.text=$"property: {bBoard.property}";

                price.text=$"price: {bBoard.price}";

                rent.text=$"rent: {bBoard.rent}";
                group.text=$"group: {bBoard.group}";
                 if(bBoard.isMortgage){
                    mortgageState.text="mortgaged";}
                    else{
                        mortgageState.text="unmortgaged";
                    }
                }}
            
                    
                    else
                    {if(eBoard.isMortgage==i){
                        GameObject o = GameObject.Instantiate(estatePrefab, generateZone.transform);
                        o.name = eBoard.property;
                    TextMeshProUGUI property = o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI price = o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI rent = o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI group= o.transform.Find("group").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI mortgageState= o.transform.Find("isMortgage").GetComponent<TextMeshProUGUI>();

                    property.text=$"property: {eBoard.property}";

                    price.text=$"price: {eBoard.price}";

                    rent.text=$"rent: {eBoard.rent}";
                    group.text=$"color: {eBoard.group}";
                    if(eBoard.isMortgage){
                    mortgageState.text="mortgaged";}
                    else{
                        mortgageState.text="unmortgaged";
                    }
                    }
                    }
                    }
            
            


        }}

    private IEnumerator FadeOut(CanvasGroup cg)
    {
        float duration = 0.25f;
        float elapsedTime = 0f;

        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (elapsedTime < duration)
        {
            cg.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cg.alpha = 0;
        isbanking=false;
     
      
    }
        private IEnumerator behaviourPanelDisplay()
    {
        isResult = false;
        yield return StartCoroutine(FadeIn(interactionCanvasGroup));
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut(interactionCanvasGroup));
        behaviourPanel.SetActive(false);
    }
   
    void quitBehaviour(){
         StartCoroutine(FadeOut(interactionCanvasGroup));

    }
    
    void confirm(string i){
        if (operationList == null)
    {
        Debug.LogError("operationList 未初始化！");
        return;
    }
        foreach (Transform child in generateZone.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
                Debug.Log($"已添加 {child.name} 进入操作列表");
                operationList.Add(child.name); 
            }
        }
        switch(i){
            case "makeSell":
            foreach( string str in operationList){
                foreach(Board board in mapList){

                    if(board.property==str){
                        estateBoard eBoard = board as estateBoard;
                        if(eBoard==null){
                        BuyableBoard bBoard=board as BuyableBoard;
                        gameBehaviour.SellBuyableBoard(player,bBoard);}
                        else{
                            gameBehaviour.SellEstateBoard(player,eBoard);
                            
                        }


                    
                    }
                }

            }


                 break;
                        
            case "makeMortgage":
               foreach( string str in operationList){
                foreach(Board board in mapList){
                    if(board.property==str){
                        estateBoard eBoard = board as estateBoard;
                        if(eBoard==null){
                        BuyableBoard bBoard=board as BuyableBoard;
                        gameBehaviour.mortageBuyableBoard(player,bBoard);}
                        else{
                            gameBehaviour.mortageEstateBoard(player,eBoard);
                            
                        }

                        
                    }
                }

            }

            break;
            
            case "makeRemdeem":
               foreach( string str in operationList){
                foreach(Board board in mapList){
                    if(board.property==str){
                        estateBoard eBoard = board as estateBoard;
                        if(eBoard==null){
                        BuyableBoard bBoard=board as BuyableBoard;
                        gameBehaviour.remdeemBuyableBoard(player,bBoard);}
                        else{
                            gameBehaviour.remdeemEstateBoard(player,eBoard);
                            
                        }

                        
                    }
                }

            }

           break;
           default:
        Debug.LogWarning($"bankPanelConfirmError: {i}");
        break;
            
        }
        

    }
}

