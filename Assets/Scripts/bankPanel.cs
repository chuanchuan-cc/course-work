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
  public TextMeshProUGUI brtext;


    

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


public void showbankruptPanel(Player p,int i){
    brtext.text=$"lack of cash, {p.name}, you need at least {i}$";
    bankruptPanel.SetActive(true);
    StartCoroutine(sbp(p,i));


}
private IEnumerator sbp(Player p,int i){
    Player saveplayer=player;
    player=p;
    StartCoroutine(FadeIn(brCanvasGroup));
    brmortgageButton.onClick.AddListener(mortgage);
    brSellButton.onClick.AddListener(sell);
    yield return new WaitUntil(()=>p.playerData.money>i);
    StartCoroutine(FadeOut(brCanvasGroup));
    player=saveplayer;
    bankruptPanel.SetActive(false);

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
        generateSellableAssets();
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
            
            foreach (Transform child in generateZone.transform)
            {
            Destroy(child.gameObject);
            }

        
            foreach(Board _board in player.playerData.assetsList){
                estateBoard eBoard = _board as estateBoard;{
                    if(eBoard==null){
                        
                BuyableBoard bBoard=_board as BuyableBoard;
                if(bBoard.isMortgage==i){
                
                GameObject o = GameObject.Instantiate(estatePrefab, generateZone.transform);
                o.name = bBoard.property;
                
                TextMeshProUGUI property= o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
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
                    Toggle toggle= o.GetComponent<Toggle>();
                    Image bgImage= o.GetComponent<Image>();
                    Color originalColor= bgImage.color;
                    Color highlightColor= new Color(254f/255f,225f/255f,131f/255f);
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color = isOn ? highlightColor : originalColor;});   
                }}
            
                    
                    else
                    {if(eBoard.isMortgage==i){
                        GameObject o = GameObject.Instantiate(estatePrefab, generateZone.transform);
                        o.name = eBoard.property;
                    TextMeshProUGUI property = o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
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
                    Toggle toggle= o.GetComponent<Toggle>();
                    Image bgImage= o.GetComponent<Image>();
                    Color originalColor= bgImage.color;
                    Color highlightColor= new Color(254f/255f,225f/255f,131f/255f);
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color = isOn ? highlightColor : originalColor;});    
                    }
                    }
                    }
            
            


        }}

        private void generateSellableAssets(){
            foreach (Transform child in generateZone.transform)
            {
            Destroy(child.gameObject);
            }
            
            List<estateBoard> el=new List<estateBoard>();
            foreach(Board _board in player.playerData.assetsList){
             
            estateBoard eB = _board as estateBoard;
            if(eB!=null){
                
            el.RemoveAll(old => old.group == eB.group && old.improvedLevel < eB.improvedLevel);

            bool hasHigher = el.Exists(old => old.group == eB.group && old.improvedLevel > eB.improvedLevel);
            if (!hasHigher) {
                el.Add(eB);
            }
                    
                
            
            
            
            
            }
            else{
           

            BuyableBoard bBoard = _board as BuyableBoard;
                GameObject o = GameObject.Instantiate(estatePrefab, generateZone.transform);
                o.name = bBoard.property;
                
                TextMeshProUGUI property= o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
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
                    Toggle toggle= o.GetComponent<Toggle>();
                    Image bgImage= o.GetComponent<Image>();
                    Color originalColor= bgImage.color;
                    Color highlightColor= new Color(254f/255f,225f/255f,131f/255f);
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color = isOn ? highlightColor : originalColor;});    
                    
                    
                    }
                
            
            
            
            
            
            }
          
            
            foreach(estateBoard eBoard in el){
                    GameObject o = GameObject.Instantiate(estatePrefab, generateZone.transform);
                    o.name = eBoard.property;
                    TextMeshProUGUI property = o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
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
                    Toggle toggle= o.GetComponent<Toggle>();
                    Image bgImage= o.GetComponent<Image>();
                    Color originalColor= bgImage.color;
                    Color highlightColor= new Color(254f/255f,225f/255f,131f/255f);
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color = isOn ? highlightColor : originalColor;});  
            }
            
            
            
            
            
            }



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

        return;
    }
    operationList.Clear();
        foreach (Transform child in generateZone.transform)
        {
            Toggle toggle = child.GetComponent<Toggle>();
            if (toggle != null && toggle.isOn)
            {
    
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
                        gameBehaviour.SellBuyableBoard(bBoard);}
                        else if(eBoard.improvedLevel!=0){
                            gameBehaviour.tearBuilding(eBoard);
                            
                        }
                        else
                        gameBehaviour.SellEstateBoard(eBoard);


                    
                    }
                }

            }

                generateSellableAssets();
                break;
                        
            case "makeMortgage":
               foreach( string str in operationList){
                foreach(Board board in mapList){
                    if(board.property==str){
                        estateBoard eBoard = board as estateBoard;
                        if(eBoard==null){
                        BuyableBoard bBoard=board as BuyableBoard;
                        gameBehaviour.mortageBuyableBoard(bBoard);}
                        else{
                            gameBehaviour.mortageEstateBoard(eBoard);
                            
                        }

                        
                    }
                }

            }
            generateAssets(false);
            break;
            
            case "makeRemdeem":
               foreach( string str in operationList){
                foreach(Board board in mapList){
                    if(board.property==str){
                        estateBoard eBoard = board as estateBoard;
                        if(eBoard==null){
                        BuyableBoard bBoard=board as BuyableBoard;
                        gameBehaviour.remdeemBuyableBoard(bBoard);}
                        else{
                            gameBehaviour.remdeemEstateBoard(eBoard);
                            
                        }

                        
                    }
                }

            }
           generateAssets(true);
           break;
           default:

        break;
            
        }
        

    }
}

