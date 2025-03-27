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
        isbanking=false;
    }
    public void setPlayer(Player _player){
        player=_player;
    }


 public void ShowPanel()
    {
       
       
        choosePanel.SetActive(true);
        


        StartCoroutine(FadeIn());

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
        


        StartCoroutine(behaviourFadeIn());
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
        StartCoroutine(FadeOut());
    }
    private IEnumerator PanelDisplay()
    {
        isResult = false;
        yield return StartCoroutine(FadeIn());
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut());
        choosePanel.SetActive(false);
    }

    void SetResult(bool result)
    {
       
        isResult = true;
        callback?.Invoke(result);
    }

    private IEnumerator FadeIn()
    {
        isbanking=true;
     

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
        isbanking=false;
     
      
    }
        private IEnumerator behaviourPanelDisplay()
    {
        isResult = false;
        yield return StartCoroutine(behaviourFadeIn());
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(behaviourFadeOut());
        behaviourPanel.SetActive(false);
    }
    private IEnumerator behaviourFadeIn()
    {
        isbanking=true;
     

        float duration = 0.25f;
        float elapsedTime = 0f;

        interactionCanvasGroup.interactable = true;
        interactionCanvasGroup.blocksRaycasts = true;

        while (elapsedTime < duration)
        {  
            interactionCanvasGroup.alpha = Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        interactionCanvasGroup.alpha = 1;
       
    }

    private IEnumerator behaviourFadeOut()
    {
        float duration = 0.25f;
        float elapsedTime = 0f;

        interactionCanvasGroup.interactable = false;
        interactionCanvasGroup.blocksRaycasts = false;

        while (elapsedTime < duration)
        {
            interactionCanvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        interactionCanvasGroup.alpha = 0;
        isbanking=false;
      
    }
    void quitBehaviour(){
         StartCoroutine(behaviourFadeOut());

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

