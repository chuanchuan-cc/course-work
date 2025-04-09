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
    public bool isResult=false;
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
  public bool isInteracting=false;
  public TextMeshProUGUI estiMoney;
  private bool isBankrupting=false;
  public Player saveplayer;
  public int brmoney;
  public string brmessage;
  public Button brQuitButton;

 public bool isLackCash=false;
public Sprite[] levelSprites;


    

    void Start()
    {
       if (bankCanvasGroup == null){
            bankCanvasGroup=choosePanel.GetComponent<CanvasGroup>();
       
            if (bankCanvasGroup == null)
            {
                bankCanvasGroup=choosePanel.AddComponent<CanvasGroup>(); 
            }
       
       }
       if (interactionCanvasGroup == null){
            interactionCanvasGroup=behaviourPanel.GetComponent<CanvasGroup>();
       
            if (interactionCanvasGroup == null)
            {
                interactionCanvasGroup=behaviourPanel.AddComponent<CanvasGroup>(); 
            }
            
       
       }
       if (brCanvasGroup == null){
            brCanvasGroup=bankruptPanel.GetComponent<CanvasGroup>();
       
            if (brCanvasGroup == null)
            {
                brCanvasGroup=bankruptPanel.AddComponent<CanvasGroup>(); 
            }
            
       
       }
         
       mapList=RunGame.mapList;
       gameBehaviour=GameObject.Find("BehaviourPool").GetComponent<GameBehaviour>();



        ClosePanel();
    }
    public void setmap(List<Board> i){
        mapList=i;

    }

public void ClosePanel()
{
    HideVisually(choosePanel.GetComponent<CanvasGroup>());
    HideVisually(behaviourPanel.GetComponent<CanvasGroup>());
    HideVisually(bankruptPanel.GetComponent<CanvasGroup>());
    isbanking=false;
}

private void HideVisually(CanvasGroup cg)
{
    if (cg != null)
    {
        cg.alpha=0f;
        cg.interactable=false;
        cg.blocksRaycasts=false;
    }
}


    public void setPlayer(Player _player){
        player=_player;
    }


public void showbankruptPanel(Player p,int i){
    setPlayer(p);

    brQuitButton.gameObject.SetActive(false);
 
    
    isBankrupting=true;
    

    
    brtext.text=$"lack of cash, {p.name}, you need at least {i}$";
    brmoney=i-p.playerData.money;
    
    brmessage=$"{p.name}, you need to raise more {i}$";

    StartCoroutine(FadeIn(brCanvasGroup));
    brmortgageButton.onClick.AddListener(mortgage);
    brSellButton.onClick.AddListener(sell);


}

public void showLackOfCashPanel(Player p,int i){
    isLackCash=true;
    setPlayer(p);
    brQuitButton.onClick.AddListener(ClosePanel);

    brQuitButton.gameObject.SetActive(true);
    if (brCanvasGroup == null){
            brCanvasGroup=bankruptPanel.GetComponent<CanvasGroup>();
       
            if (brCanvasGroup == null)
            {
                brCanvasGroup=bankruptPanel.AddComponent<CanvasGroup>(); 
            }
            
       
       }
    
    

    
    brtext.text=$"lack of cash, {p.name}, you need at least {i}$";
    brmoney=i-p.playerData.money;
    
    brmessage=$"{p.name}, you need to raise more {i}$";

    StartCoroutine(FadeIn(brCanvasGroup));
    brmortgageButton.onClick.AddListener(mortgage);
    brSellButton.onClick.AddListener(sell);


}
// private IEnumerator sbp(Player p,int i){
    
   // yield return new WaitUntil(()=>isInteracting);
  // StartCoroutine(FadeOut(brCanvasGroup));
 // player=saveplayer;
 // bankruptPanel.SetActive(false);

// }

 public void ShowPanel()
    {
       

        


        StartCoroutine(FadeIn(bankCanvasGroup));

        sellButton.onClick.AddListener(sell);
        mortgageButton.onClick.AddListener(mortgage);
        remdeemButton.onClick.AddListener(remdeem);
        quitButton.onClick.AddListener(quit);
        
    }
    public void ShowBehaviourPanel(string _message)
    {
        if(isBankrupting){
        quitButton.interactable=false;
        confirmButton.interactable=false;
        }else{
            quitButton.interactable=true;
        confirmButton.interactable=true;

        }
        operationList.Clear();
        
      


        message.text=_message;
        


        StartCoroutine(FadeIn(interactionCanvasGroup));
        behaviourQuit.onClick.AddListener(quitBehaviour);
          while (generateZone.transform.childCount > 0)
    {
        GameObject.DestroyImmediate(generateZone.transform.GetChild(0).gameObject);
    }
        

    }
    private void sell(){
        isInteracting=true;
        if(isBankrupting)
        ShowBehaviourPanel(brmessage);
        else
        ShowBehaviourPanel("which estate you are willing to sell");
        generateSellableAssets();
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(()=>confirm("makeSell"));
     
        isInteracting=false;
      
    }
    private void mortgage(){
        isInteracting=true;
        if(isBankrupting)
        ShowBehaviourPanel(brmessage);
        else
        ShowBehaviourPanel("which estate you are willing to mortage");
        generateAssets(false);
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(()=>confirm("makeMortgage"));
      
        isInteracting=false;
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
        isResult=false;
        yield return StartCoroutine(FadeIn(bankCanvasGroup));
        yield return new WaitUntil(() => isResult);
        yield return StartCoroutine(FadeOut(bankCanvasGroup));
        choosePanel.SetActive(false);
    }

    void SetResult(bool result)
    {
       
        isResult=true;
        callback?.Invoke(result);
    }

    private IEnumerator FadeIn(CanvasGroup cg)
    {
        isbanking=true;
     

        float duration=0.25f;
        float elapsedTime=0f;

        cg.interactable=true;
        cg.blocksRaycasts=true;

        while (elapsedTime < duration)
        {  
            cg.alpha=Mathf.Lerp(0, 1, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
      
        cg.alpha=1;
       
    }
private void generateAssets(bool i){
            if(isBankrupting){
            checkComfirm(false);
            estiMoney.text=$"{checkPrice(false)}/{brmoney}";

            }
            else
            estiMoney.text=$"{checkPrice(false)}";
            
            foreach (Transform child in generateZone.transform)
            {
            Destroy(child.gameObject);
            }

        
            foreach(Board _board in player.playerData.assetsList){
                estateBoard eBoard=_board as estateBoard;{
                    if(eBoard==null){
                        
                BuyableBoard bBoard=_board as BuyableBoard;
                if(bBoard.isMortgage==i){
                
                GameObject o=GameObject.Instantiate(estatePrefab, generateZone.transform);
                o.name=bBoard.property;
                
                TextMeshProUGUI property= o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI group= o.transform.Find("group").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI mortgageState= o.transform.Find("isMortgage").GetComponent<TextMeshProUGUI>();
                Image buildImg=o.transform.Find("buildingNum").GetComponent<Image>();
                buildImg.gameObject.SetActive(false);

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
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color=isOn ? highlightColor : originalColor;
                    if(isBankrupting){
                    checkComfirm(false);
                    estiMoney.text=$"{checkPrice(false)}/{brmoney}";

                    }
                    else if(isLackCash)
                    estiMoney.text=$"{checkPrice(false)}/{brmoney}";

                    else
                    estiMoney.text=$"{checkPrice(false)}";
                    
                    });   
                }}
            
                    
                    else
                    {if(eBoard.isMortgage==i){
                        GameObject o=GameObject.Instantiate(estatePrefab, generateZone.transform);
                        o.name=eBoard.property;
                    TextMeshProUGUI property=o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI group= o.transform.Find("group").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI mortgageState= o.transform.Find("isMortgage").GetComponent<TextMeshProUGUI>();
                    Image buildImg=o.transform.Find("buildingNum").GetComponent<Image>();
                    

                    property.text=$"property: {eBoard.property}";

                    price.text=$"price: {eBoard.price}";

                    rent.text=$"rent: {eBoard.rent}";
                    group.text=$"color: {eBoard.group}";
                    if(eBoard.isMortgage){
                    mortgageState.text="mortgaged";}
                    else{
                        mortgageState.text="unmortgaged";
                    }
                    switch(eBoard.improvedLevel){
                    case 1:
                    buildImg.sprite=levelSprites[0];
                    break;
                    case 2:
                    buildImg.sprite =levelSprites[1];
                    break;
                    case 3:
                    buildImg.sprite =levelSprites[2];
                    break;
                    case 4:
                    buildImg.sprite =levelSprites[3];
                    break;
                    case 5:
                    buildImg.sprite =levelSprites[4];
                    break;
                    default:
                    buildImg.sprite=null;
                    buildImg.gameObject.SetActive(false);
                    break;



                    }
                    Toggle toggle= o.GetComponent<Toggle>();
                    Image bgImage= o.GetComponent<Image>();
                    Color originalColor= bgImage.color;
                    Color highlightColor= new Color(254f/255f,225f/255f,131f/255f);
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color=isOn ? highlightColor : originalColor;
                    if(isBankrupting){
                    checkComfirm(false);
                    estiMoney.text=$"{checkPrice(false)}/{brmoney}";

                    }
                    else if(isLackCash)
                    estiMoney.text=$"{checkPrice(false)}/{brmoney}";
                    else
                    estiMoney.text=$"{checkPrice(false)}";
                    });    
                    }
                    }
                    }
            
            


        }}

        private void generateSellableAssets(){
            if(isBankrupting){
            checkComfirm(false);
            estiMoney.text=$"{checkPrice(false)}/{brmoney}";

            }
            else
            estiMoney.text=$"{checkPrice(false)}";
            foreach (Transform child in generateZone.transform)
            {
            Destroy(child.gameObject);
            }
            
            List<estateBoard> el=new List<estateBoard>();
            foreach(Board _board in player.playerData.assetsList){
             
            estateBoard eB=_board as estateBoard;
            if(eB!=null){
                
            el.RemoveAll(old => old.group == eB.group && old.improvedLevel < eB.improvedLevel);

            bool hasHigher=el.Exists(old => old.group == eB.group && old.improvedLevel > eB.improvedLevel);
            if (!hasHigher) {
                el.Add(eB);
            }
                    
                
            
            
            
            
            }
            else{
           

            BuyableBoard bBoard=_board as BuyableBoard;
                GameObject o=GameObject.Instantiate(estatePrefab, generateZone.transform);
                o.name=bBoard.property;
                
                TextMeshProUGUI property= o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI price= o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI rent= o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI group= o.transform.Find("group").GetComponent<TextMeshProUGUI>();
                TextMeshProUGUI mortgageState= o.transform.Find("isMortgage").GetComponent<TextMeshProUGUI>();
                Image buildImg=o.transform.Find("buildingNum").GetComponent<Image>();
                buildImg.gameObject.SetActive(false);
                
                
                

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
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color=isOn ? highlightColor : originalColor;
                    if(isBankrupting){
                    checkComfirm(true);
                    estiMoney.text=$"{checkPrice(true)}/{brmoney}";

                    }
                    else if(isLackCash)
                    estiMoney.text=$"{checkPrice(true)}/{brmoney}";
                    else
                    estiMoney.text=$"{checkPrice(true)}";
                    
                    
                    
                    });    
                    
                    
                    }
                
            
            
            
            
            
            }
          
            
            foreach(estateBoard eBoard in el){
                    GameObject o =GameObject.Instantiate(estatePrefab, generateZone.transform);
                    o.name=eBoard.property;
                    TextMeshProUGUI property=o.transform.Find("property").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI price=o.transform.Find("price").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI rent=o.transform.Find("rent").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI group=o.transform.Find("group").GetComponent<TextMeshProUGUI>();
                    TextMeshProUGUI mortgageState=o.transform.Find("isMortgage").GetComponent<TextMeshProUGUI>();
                    Image buildImg=o.transform.Find("buildingNum").GetComponent<Image>();

                    property.text=$"property: {eBoard.property}";

                    price.text=$"price: {eBoard.price}";

                    rent.text=$"rent: {eBoard.rent}";
                    group.text=$"color: {eBoard.group}";
                    if(eBoard.isMortgage){
                    mortgageState.text="mortgaged";}
                    else{
                        mortgageState.text="unmortgaged";
                    }
                    switch(eBoard.improvedLevel){
                        case 1:
                        buildImg.sprite=levelSprites[0];
                        break;
                        case 2:
                        buildImg.sprite =levelSprites[1];
                        break;
                        case 3:
                        buildImg.sprite =levelSprites[2];
                        break;
                        case 4:
                        buildImg.sprite =levelSprites[3];
                        break;
                        case 5:
                        buildImg.sprite =levelSprites[4];
                        break;
                        default:
                        buildImg.sprite=null;
                        buildImg.gameObject.SetActive(false);
                        break;



                    }
                    Toggle toggle= o.GetComponent<Toggle>();
                    Image bgImage= o.GetComponent<Image>();
                    Color originalColor= bgImage.color;
                    Color highlightColor= new Color(254f/255f,225f/255f,131f/255f);
                    toggle.onValueChanged.AddListener((isOn)=>{bgImage.color=isOn ? highlightColor : originalColor;
                    if(isBankrupting){
                    checkComfirm(true);
                    estiMoney.text=$"{checkPrice(true)}/{brmoney}";

                    }
                    else if(isLackCash)
                    estiMoney.text=$"{checkPrice(true)}/{brmoney}";
                    else
                    estiMoney.text=$"{checkPrice(true)}";
                    });  
            }
            
            
            
            
            
            }
   
   
   private void checkComfirm(bool b){
    int p =checkPrice(b);
    confirmButton.interactable=p>=brmoney;
   }
   
   private int checkPrice(bool b)
{
    int p=0;
    foreach (Transform child in generateZone.transform)
    {
        Toggle toggle=child.GetComponent<Toggle>();
        if (toggle==null||!toggle.isOn) continue;
        string propName=child.name;
        Board board=mapList.Find(b => b.property==propName);
        if (board is estateBoard eB){
            if(eB.improvedLevel==0)
            p+=b?eB.price:eB.price/2;
            else{
                p+=b?gameBehaviour.getprice(eB,eB.improvedLevel-1):gameBehaviour.getprice(eB,eB.improvedLevel-1)/2;
            }
        }
        else if (board is BuyableBoard bB){
            p+=b?bB.price:bB.price/2;
        }}
        return p;
    
}





    private IEnumerator FadeOut(CanvasGroup cg)
    {
        float duration=0.25f;
        float elapsedTime=0f;

        cg.interactable=false;
        cg.blocksRaycasts=false;

        while (elapsedTime < duration)
        {
            cg.alpha=Mathf.Lerp(1, 0, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cg.alpha=0;
        isbanking=false;
     
      
    }
        private IEnumerator behaviourPanelDisplay()
    {
        isResult=false;
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
            Toggle toggle=child.GetComponent<Toggle>();
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
                        estateBoard eBoard=board as estateBoard;
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
            if(isBankrupting){
                isBankrupting=false;
                quitButton.interactable=true;
                ClosePanel();
                
                    
                }
                if(isLackCash){
                    ClosePanel();
                    isLackCash=false;
                }

                generateSellableAssets();
                break;
                        
            case "makeMortgage":
               foreach( string str in operationList){
                foreach(Board board in mapList){
                    if(board.property==str){
                        estateBoard eBoard=board as estateBoard;
                        if(eBoard==null){
                        BuyableBoard bBoard=board as BuyableBoard;
                        gameBehaviour.mortageBuyableBoard(bBoard);}
                        else{
                            gameBehaviour.mortageEstateBoard(eBoard);
                            
                        }
                    

                        
                    }
                }

            }
            if(isBankrupting){
                isBankrupting=false;
      
                ClosePanel();
                quitButton.interactable=true;
                    
                }if(isLackCash){
                ClosePanel();
                isLackCash=false;}
            generateAssets(false);
            break;
            
            case "makeRemdeem":
               foreach( string str in operationList){
                foreach(Board board in mapList){
                    if(board.property==str){
                        estateBoard eBoard=board as estateBoard;
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

