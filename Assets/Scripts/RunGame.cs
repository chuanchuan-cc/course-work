using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using Newtonsoft.Json;




public class RunGame : MonoBehaviour
{
    public static RunGame instance;
    public bool isLoadGame;
    public static Bank bank;
    public static List<Player> playersList;
    public static List<Board> mapList;
    public static List<Card> luckCards;
    public static List<Card> opportunityCards;
    public static bool isAI;
    public int luckNo=0;
    public int OpportunityNo=0;

    public bool keepGame = true;
    public Player currentPlayer;
    public CGcontrol cgControl;

    public int point;
    public Button DiceButton;
    public Button NextButton;
    public bool isEffectiveDice = false;

    public GameObject playersPool;
    private GameBehaviour gameBehaviour;
    

    private int diceRolls;
    public CardUI cardUI; 
    public bool isbehavior;
    public bool isNext;
    public broadcast Broadcast;
    public int freeParkMoney=0;
    public GameObject dashBoard;
    public dashBoardConstructor BoardConstructor;
    public playerInteractionPanel interactionPanel;
    public bool isChecking=false;
    public bool isProcessingCard=false;
    public bool isAuction=false;
    public Button BankButton;
    public int difficulty;
    bool isApplyCard=false;
    public TileGenerator generator;
    public bankPanel bankpanel;
    public testMenus testmenus;
    public Button buildingButton;
    private int cheatStep=0;
    public CameraController cameraController;
    public Button viewButton;
    public Button menusButton;
    public GameObject menus;
    public Button menusQuit;
    public Button menusSave;
    public Button menusBack;
    public int oldPosNo;
 


    private SaveData cachedSaveData; 
    


    //测试用玩家

    int roll;

   void Awake()
{
    instance = this;
    isLoadGame = PlayerPrefs.GetInt("isLoadGame", 0) == 1;
        PlayerPrefs.SetInt("isLoadGame", 0);
    PlayerPrefs.Save();
   
        

        

        
        DiceButton.interactable = false;
        gameBehaviour = GameObject.Find("BehaviourPool").GetComponent<GameBehaviour>();
        cameraController= GameObject.Find("Main Camera").GetComponent<CameraController>();

        dashBoard = GameObject.Find("DashBoard");
        BoardConstructor=dashBoard.GetComponent<dashBoardConstructor>();




        
        playersPool = GameObject.Find("PlayersPool");
        


        
        
        bank=new Bank();

            bankpanel=GameObject.Find("bankPanel").GetComponent<bankPanel>();
        if(bankpanel==null){
            Debug.Log("bankpanel初始化失败");
        }

        
        
       
       
        
       

        // 自动查找并绑定 CardUI
        cardUI = FindObjectOfType<CardUI>();

        //绑定操作交互面板

        interactionPanel=GameObject.Find("interactionPanel").GetComponent<playerInteractionPanel>();
        if(interactionPanel==null)Debug.Log("can't find interaction panel");
        else Debug.Log("already find interaction panel");
        
        //绑定bank
        BankButton=GameObject.Find("BankButton").GetComponent<Button>();


        //绑定CG控制器
        cgControl=  FindObjectOfType<CGcontrol>();



        Broadcast = GameObject.Find("broadcast").GetComponent<broadcast>();

    


        DiceButton.onClick.AddListener(ThrowDice);
        NextButton.onClick.AddListener(next);
        buildingButton.onClick.AddListener(build);

        

    
    NextButton.gameObject.SetActive(false);


}

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{



     if (isLoadGame)
    {
        Debug.Log("读取游戏");
        string path = PlayerPrefs.GetString("savePath");
    if (!System.IO.File.Exists(path))
    {
        Debug.LogWarning("没有找到保存文件！");
        return;
    }

    string json = System.IO.File.ReadAllText(path);
SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json, new JsonSerializerSettings
{
    TypeNameHandling = TypeNameHandling.Auto
});
    luckCards=saveData.lCards;
    luckNo=saveData.luckNo;
    opportunityCards=saveData.oCards;
    OpportunityNo=saveData.OpportunityNo;
    mapList = saveData.allBoards;
    point=saveData.cPoint;
    isAI=saveData.isai;
    difficulty=saveData.diff;



    freeParkMoney = saveData.freeParkingMoney;

    playersList=new List<Player>(saveData.allPlayers.Count);
    foreach(PlayerData pd in saveData.allPlayers){
        foreach (Transform child in playersPool.transform)
        {
            Player player = child.GetComponent<Player>();
            if(playersList.Count>=saveData.allPlayers.Count)
            player.gameObject.SetActive(false);
            else{
            
            if (player != null &&player.gameObject.name==pd.name)
            {
                
                
   
        
            player.loadPlayer(pd);


            player.playerData.assetsList.Clear();

            foreach(Board b in mapList){
                if(player.playerData.assetsName.Contains(b.property)){
                    estateBoard eb= b as estateBoard;
                    if(eb!=null){
                        gameBehaviour.AddProperty(player,eb);
                        Debug.Log($"已将{eb.property}添加至玩家资产列表");

                    }else{
                        BuyableBoard bb= b as BuyableBoard;
                        if(bb!=null){
                            gameBehaviour.AddBuyable(player,bb);
                            Debug.Log($"已将{bb.property}添加至玩家资产列表");
                        }
                    }
                }
            }
            
            
            playersList.Add(player);
            }
            }}


    }




    
    }
    else
    {   
        // read the input from last scene

        
        isAI = PlayerPrefs.GetInt("IsAI", 0) == 1;
        difficulty=PlayerPrefs.GetInt("difficulty",0);
        int playerNumber = PlayerPrefs.GetInt("PlayerNumber", 1);
        playerNumber=(isAI)? playerNumber+1:playerNumber;
        



        //测试用
        isAI=true;
        difficulty=1;
        playerNumber=2;



        point = 0;
    //initialize player


        playersList= new List<Player>(playerNumber);
            foreach (Transform child in playersPool.transform)
        {
            Player player = child.GetComponent<Player>();
            if(playersList.Count>=playerNumber)
            player.gameObject.SetActive(false);
            else{
            
            if (player != null)
            {
                Debug.Log("find Player: " + child.name);
                
                
   
        
            player.InitializePlayer(player.gameObject.name);
            
            playersList.Add(player);
            }
            }}
        

        if(isAI) playersList[playerNumber-1].playerData.isAI=true;


    
    
    //initialize the map and card
    string cardPath=(PlayerPrefs.GetString("cardPath")==null)? PlayerPrefs.GetString("cardPath"):Application.dataPath+"/Resources/card/testCard.xlsx";
    string mapPath=(PlayerPrefs.GetString("mapPath")==null)?PlayerPrefs.GetString("mapPath"):Application.dataPath+"/Resources/map/testMap.xlsx";;
  
    if(mapPath== null){
        Debug.LogError("mapPath is null");
    }else{
    try{mapList=BoardLoader.LoadBoards(mapPath);
    }catch(System.Exception e){
        Debug.LogError("Error happen when initialize the map:"+e.Message);
    }

    }
    if(cardPath== null){
        Debug.LogError("cardPath is null");
    }else{
        try{
    (luckCards,opportunityCards)=CardLoader.LoadCards(cardPath);
    } catch(System.Exception e){
        Debug.LogError("Error happen when initialize the card:"+e.Message);
    }
     
    Shuffle(luckCards);
    Shuffle(opportunityCards);
    }
    }

             foreach(Player player in playersList){
            BoardConstructor.CreateChildren(player);
        }
                
    
   
    

    


    generator=GameObject.Find("Map").GetComponent<TileGenerator>();
    generator.GenerateMapFromList(mapList);
    BankButton.onClick.AddListener(showbankPanel);
    BankButton.interactable=false;
    viewButton.onClick.AddListener(changeCameraMode);
    viewButton.interactable=false;
    menusButton.onClick.AddListener(showMenus);
    menusButton.interactable=false;
    menusBack.onClick.AddListener(closeMenus);
    menusQuit.onClick.AddListener(quitGame);
    menusSave.onClick.AddListener(SaveGame);

    bankpanel.ClosePanel();



    
    //start
    
    StartCoroutine(GameLoop());
    bankpanel.setmap(mapList);

    testmenus.setmap(mapList);

   
    

}
void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            testmenus.cheating(currentPlayer);
        }
        StartCoroutine(CheatingUppdate());


    }





    IEnumerator GameLoop()
{



    
    diceRolls=0;
    
    while (keepGame)
    {
        menus.gameObject.SetActive(false);
        
        cardUI.HideCard();
        isbehavior=false;
        isNext=false;
        DiceButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        NextButton.interactable=false;
        buildingButton.gameObject.SetActive(false);
        if(cameraController.canBeDragging){
        changeCameraMode();
        yield return new WaitUntil(()=>!cameraController.isCameraMoving);
        }
        AutoSaveGame();

        

        
        

       


        
        isEffectiveDice=false;

        currentPlayer = playersList[point];

        PlayerDisplay playerDisplay=dashBoard.transform.Find(currentPlayer.name).GetComponent<PlayerDisplay>();


        Broadcast.showBroad(currentPlayer);
        yield return new WaitForSeconds(1f);
        Broadcast.closeBroad(currentPlayer);
        yield return new WaitUntil(()=>!Broadcast.isBroadcasting);
        BankButton.interactable=true;
        viewButton.interactable=true;
        menusButton.interactable=true;


   
        point = (point + 1) % playersList.Count;
        
        if (currentPlayer.playerData.freezeTurn > 0)
        {
            currentPlayer.playerData.freezeTurn -= 1;
            yield return new WaitForSeconds(0.5f);
            if(currentPlayer.playerData.isAI)continue;
            else{
            NextButton.interactable=true;
            yield return new WaitUntil(()=>isNext);
            continue;
            }
        }
        if(!currentPlayer.playerData.isAI){
            DiceButton.gameObject.SetActive(true);
            NextButton.gameObject.SetActive(false);
            DiceButton.interactable = true;
        }else{
            AIRoll();
        }
        
        yield return new WaitUntil(() => isEffectiveDice);

        //作弊菜单
        roll=(cheatStep!=0)? cheatStep: roll;
        oldPosNo=currentPlayer.playerData.positionNo;
        int PassGoNum=(roll>0)? roll:0;
        
        
        
        if(roll==-1){
            cgControl.CGDisplay("GoToJail");
            gameBehaviour.GoToJail(currentPlayer);
            yield return new WaitUntil(()=>!currentPlayer.isMoving);
            
        }else if (!currentPlayer.isMoving) 
        {
         
           currentPlayer.Move(roll);




        }
        

        DiceButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        NextButton.interactable=false;


        yield return new WaitUntil(() => !currentPlayer.isMoving); 
        
        
  
       


        if (oldPosNo+PassGoNum>=40)
        {
            Debug.Log("通过起点加钱");
               gameBehaviour.AddMoney(currentPlayer, 200);
               bank.money-=200;
            
            currentPlayer.playerData.circle += 1;
        }
    


        StartCoroutine(check(currentPlayer));
      

        
        if (currentPlayer.playerData.isBankrupt)
        {
            playersList.RemoveAt((point-1)%playersList.Count);
        }

        if (playersList.Count == 1)
        {
            break;
        }
      
        yield return new WaitUntil(()=>!isChecking);
        
        foreach (Player p in playersList)
        {playerUpdate(p);}

        
        
        
        NextButton.interactable=true;
        Debug.Log($"{currentPlayer.playerData.name}当前圈数为{currentPlayer.playerData.circle}");

        yield return new WaitUntil(() => isNext||currentPlayer.playerData.isAI);

        

    }
}
public void playerUpdate(Player p){

    PlayerDisplay display = dashBoard.transform.Find(p.name).GetComponent<PlayerDisplay>();
    if (display != null)
    {
        display.UpdateDisplay(p);
  
    }
    else
    {
        Debug.LogError($"找不到 {p.name} 的 PlayerDisplay！");
    }

}


public void ThrowDice()
{
  
    roll=0;
    isbehavior=true;
    DiceButton.interactable = false;
    int roll1, roll2;

    roll1 = Random.Range(1, 7);
    roll2 = Random.Range(1, 7);
    roll = roll1 + roll2;

    Debug.Log($"roll1={roll1}, roll2={roll2}, roll={roll}, diceRolls={diceRolls}");
    if (roll % 2 == 0)
    {
        diceRolls++;
        if (diceRolls == 3)
        {
            roll = -1;
            diceRolls=0;
            isbehavior=false;
            isEffectiveDice = true;
            return;
        }else
        {
            DiceButton.interactable = true;
           
            return;
        }
    }else
    {
        isbehavior=false;
        isEffectiveDice = true;
        diceRolls=0;
           
    }

       
   

}
void AIRoll(){
    roll=0;
    isbehavior=true;
    DiceButton.interactable = false;
    int roll1, roll2;
    
    while(diceRolls<3){
        diceRolls+=1;
        roll1 = Random.Range(1, 7);
        roll2 = Random.Range(1, 7);
        roll = roll1 + roll2;

       
    if(roll%2!=0){
        isbehavior=false;
        isEffectiveDice = true;
        diceRolls=0;
        return;
        
    }    
    roll=-1;
    diceRolls=0;
    isbehavior=false;
    isEffectiveDice = true;
    }
}
 

 IEnumerator check(Player player)
    {
        isChecking=true;
        yield return new WaitForSeconds(0.5f);
        
        Board currentBoard = mapList[player.playerData.positionNo];
      

        // 判断是否踩到抽卡格子
        if (currentBoard.property == "Opportunity Knocks" || currentBoard.property == "Pot Luck")
        {
            Debug.Log("触发drawcard");
            StartCoroutine(DrawCard(player,currentBoard));
            yield return new WaitUntil(()=>!isProcessingCard);
            isChecking = false;
            yield break;
        }

        if (player.playerData.freeJail > 0)
        {
            player.playerData.freeJail--;
        }
        else
        {if(currentBoard.property=="Free Parking"){
            gameBehaviour.AddMoney(currentPlayer,freeParkMoney);
        }
        if(currentBoard.property=="Go to Jail"){
            gameBehaviour.GoToJail(currentPlayer);
        }
        if(currentBoard.property=="Income Tax"){
            gameBehaviour.PayMoney(currentPlayer,200);
        }
        if(currentBoard.property=="Super Tax"){
            gameBehaviour.PayMoney(currentPlayer,100);
        }
            if (currentBoard.canBeBought)
            {
                estateBoard eBoard = currentBoard as estateBoard;
                if (eBoard != null)
                {
                    yield return HandleEstate(player, eBoard);
                }
                else{
                    BuyableBoard bBoard = currentBoard as BuyableBoard;{
                        if(bBoard!=null){
                            yield return HadleBuyable(player,bBoard);
                        }
                    }
                }
            }
        }
        isChecking=false;
    }


    IEnumerator DrawCard(Player player,Board board)
    {
        isProcessingCard=true;
        bool isInteracting=false;
        
        
        Card drawnCard;
        List<Card> deck;
        if(board.property=="Pot Luck"){
            deck = luckCards;
         drawnCard = deck[luckNo];
        Debug.Log($"{player.name} 抽到了卡片: {drawnCard.description}");
        luckNo=(luckNo+1)%deck.Count;
        }
        else{
             deck =opportunityCards;
           drawnCard = deck[OpportunityNo];
        Debug.Log($"{player.name} 抽到了卡片: {drawnCard.description}");
        OpportunityNo=(OpportunityNo+1)%deck.Count;  
        }

        // 显示卡片 UI
        cardUI.ShowCard(drawnCard);


        if(drawnCard.isInteractable){
            isInteracting=true;


                 bool? userChoice=null;
                 string[] parts=drawnCard.description.Split(" or ");
            interactionPanel.ShowPanel($"are you want to {parts[1]}, instead of {parts[0]}?",(bool Result)=> 
          { userChoice=Result;
                
          });

          yield return new WaitUntil(()=>userChoice.HasValue);
               if(userChoice.Value){
                
                foreach(Board i in mapList){
                    if(i.property=="Opportunity Knocks"){

                        StartCoroutine(DrawCard(currentPlayer,i));
                   

                        yield break;


                    }

                }

                

               
                                }
                    
                   isInteracting=false;            
                           
                                
                

        }

        yield return new WaitUntil(()=>!isInteracting);


        // 处理卡片效果
        ApplyCardEffect(player, drawnCard);
        float timer = 0f;
  
    while (timer < 5f||isApplyCard)
    {
        if(currentPlayer.playerData.isAI){
            isProcessingCard=false;
            break;
        }
        if (Input.GetMouseButtonDown(0)) // 监听鼠标点击
        {
            Debug.Log("点击屏幕，立即关闭卡片 UI");
            isProcessingCard=false;
            break;
        }
        timer += Time.deltaTime;
        yield return null;   
    }
        cardUI.HideCard();
        yield return new WaitUntil(()=>!isApplyCard);
        isProcessingCard=false;
        
    }


    void ApplyCardEffect(Player player, Card card)
    {
        isApplyCard=true;
       
        if (card.isMove)
        {
            Debug.Log($"前往{card.destinationName}");
            
                foreach(Board i in mapList){
                    if(i.property==card.destinationName){
                        Debug.Log($"已探测到格子{i.property}");
                        int n=currentPlayer.playerData.positionNo;
                      if(card.isFoward)
                {
                    currentPlayer.directlyMove(i);
                    if(currentPlayer.playerData.positionNo<oldPosNo){
                        if(!card.collectGo)
                        currentPlayer.playerData.circle++;
                        
                    }

                }
                else
                {
                    currentPlayer.directlyMove(i);
                    if(currentPlayer.playerData.positionNo<oldPosNo){
                        currentPlayer.playerData.circle--;
                    }

                }
                 
                 

                    }
                
                isApplyCard=false;
                 return;
                
            
            
        }
        }
        if (card.isPay)
        {Debug.Log($"触发{card.payer}付{card.payee} {card.moneyAmount}");
            if(card.payee=="player"&&card.payer=="Bank"){
               bank.money-=card.moneyAmount;
               gameBehaviour.AddMoney(player, card.moneyAmount);
               
            }
            else if(card.payee=="player"&&card.isPayByAll){
                foreach(Player i in playersList){
                    gameBehaviour.PayMoney(i,card.moneyAmount);
                    gameBehaviour.AddMoney(player,card.moneyAmount);

                }
            }
           
            else if(card.payee=="Bank"&&card.payer=="player"){bank.money+=card.moneyAmount;
            gameBehaviour.PayMoney(player, card.moneyAmount);
            }
            
            
            
            }
        if(card.isPayFine){
        Debug.Log($"触发{player.name}付{card.moneyAmount}给免费停车");
            
            freeParkMoney+=card.moneyAmount;
            gameBehaviour.PayMoney(player,card.moneyAmount);

        }
        

        
        if (card.isJailFree)
        {
            Debug.Log($"触发给{player.name}免死金牌");
            player.playerData.freeJail += 1;
        }
        if (card.isGoJail)
        {
            Debug.Log($"触发{player.name}进监狱");
            gameBehaviour.GoToJail(player);
        }
        playerUpdate(player);
        isApplyCard=false;
        
    }
    void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        for (int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]); 
        }
    }
    public void next(){
        isNext=true;
    }
    IEnumerator auction(estateBoard eBoard){
        isAuction=true;
        int auctionPrice=eBoard.price;
        int detlaPrice=auctionPrice/10;
        int totalNum=playersList.Count-1;
        List<Player> auctionList=new List<Player>();
        for(int i=0;i<totalNum;i++){
            int t=(point+i)%playersList.Count;
            Player p =playersList[t];
            //圈数检测禁用，测试
            // if(p.playerData.circle>=1) auctionList.Add(playersList[t]);

            //开启检测
            if(p.playerData.circle>1) auctionList.Add(playersList[t]);
            else continue;
        }

            Player buyer=null;
            int n =0;
            while(auctionList.Count>0){
                Player acutionPlayer= auctionList[n];
                if (acutionPlayer.playerData.isAI==false){
            bool? userChoice=null;
                        interactionPanel.ShowPanel($"{acutionPlayer.name}, the auction price of {eBoard.property} is {auctionPrice}, are you want to buy this estate?",eBoard.group,eBoard.price,eBoard.rent,(bool isBuy)=> 
                        { userChoice=isBuy;
                      
                        });
                        if(acutionPlayer.playerData.money<auctionPrice){
                        interactionPanel.yesButton.interactable=false;}
                        yield return new WaitUntil(()=>userChoice.HasValue);
                            if(userChoice.Value){
                                auctionPrice+=detlaPrice;
                                buyer=acutionPlayer;
                                }
                               
                            else
                                auctionList.RemoveAt(n);
                }else {
                if(acutionPlayer.playerData.money>=auctionPrice&&AIauction(acutionPlayer,auctionPrice)){
                     auctionPrice+=detlaPrice;
                                buyer=acutionPlayer;

                }else{
                    auctionList.RemoveAt(n);

                }
                }
                            if(auctionList.Count==1&&buyer!=null){
                            gameBehaviour.PayMoney(buyer,auctionPrice);
                            gameBehaviour.AddProperty(buyer,eBoard);
                            generator.updateTile(eBoard);
                            isAuction=false;
                            yield break; }
                             if(auctionList.Count==0&&buyer==null){
                isAuction=false;
                yield break;
            }

                            n=(n+1) %auctionList.Count;
                                
                            
                            
                            }
                           
           
      

        }
    IEnumerator BuyableAuction(BuyableBoard bBoard){
        isAuction=true;
        int auctionPrice=bBoard.price;
        int detlaPrice=auctionPrice/10;
        int totalNum=playersList.Count-1;
        List<Player> auctionList=new List<Player>();
        for(int i=0;i<totalNum;i++){
            int t=(point+i)%playersList.Count;
            Player p =playersList[t];
            //圈数检测禁用，测试
            // if(p.playerData.circle>=1) auctionList.Add(playersList[t]);
            //开启检测
            if(p.playerData.circle>1) auctionList.Add(playersList[t]);
            else continue;
        }

            Player buyer=null;
            int n =0;
            while(auctionList.Count>0){
                Player acutionPlayer= auctionList[n];
                if (acutionPlayer.playerData.isAI==false){
            bool? userChoice=null;
                        interactionPanel.ShowPanel($"{acutionPlayer.name}, the auction price of {bBoard.property} is {auctionPrice}, are you want to buy this estate?",bBoard.group,bBoard.price,null,(bool isBuy)=> 
                        { userChoice=isBuy;
                      ;
                        });
                        if(acutionPlayer.playerData.money<auctionPrice){
                        interactionPanel.yesButton.interactable=false;}
                        yield return new WaitUntil(()=>userChoice.HasValue);
                            if(userChoice.HasValue && userChoice.Value){
                                auctionPrice+=detlaPrice;
                                buyer=acutionPlayer;
                                }
                               
                            else
                                auctionList.RemoveAt(n);
                }else {
                if(acutionPlayer.playerData.money>=auctionPrice&&AIauction(acutionPlayer,auctionPrice)){
                     auctionPrice+=detlaPrice;
                                buyer=acutionPlayer;

                }else{
                    auctionList.RemoveAt(n);

                }
                }
                            if(auctionList.Count==1&&buyer!=null){
                            gameBehaviour.PayMoney(buyer,auctionPrice);
                            gameBehaviour.AddBuyable(buyer,bBoard);
                            generator.updateTile(bBoard);
  
                            isAuction=false;
                            yield break; }
                             if(auctionList.Count==0&&buyer==null){
                isAuction=false;
                yield break;
            }

                            n=(n+1) %auctionList.Count;
                                
                            
                            
                            }
                           
           
      

        }
        
        
        
private void showbankPanel(){
    StartCoroutine(showBankPanel());

}
        


private IEnumerator showBankPanel(){
        if(DiceButton &&DiceButton.interactable ==true){
            DiceButton.interactable =false;
        }if(NextButton &&NextButton.interactable ==true){
            NextButton.interactable=false;
        }
        bankpanel.ShowPanel();
        bankpanel.setPlayer(currentPlayer);
        yield return new WaitUntil(()=>!bankpanel.isbanking);
        if(DiceButton &&DiceButton.interactable  ==false){
            DiceButton.interactable =true;
        }if(NextButton &&NextButton.interactable ==false){
            NextButton.interactable =true;
        }
        playerUpdate(currentPlayer);
        
    }

    private bool AIauction(Player player,int price){
        if(player.playerData.isAI){
            if(difficulty==0){
                return UnityEngine.Random.Range(0, 2) == 0;
            }else if (difficulty==1){
                if ((player.playerData.money-price)>=0.3*player.playerData.assetsWorth)
                return true;
                else return false;
            }else{
                //难布尔值
                return true;
            }
        }else{
            Debug.LogError($"player {player.name} is not AI");
            return false;
        }
    }
    private bool AIBuyProperty(Player player,int price){
        if(player.playerData.isAI){
            if(difficulty==0){
                return UnityEngine.Random.Range(0, 2) == 0;
            }else if (difficulty==1){
                              if ((player.playerData.money-price)>=0.3*player.playerData.assetsWorth)
                return true;
                else return false;
            }else{
                //难布尔值
                return true;
            }
        }else{
            Debug.LogError($"player {player.name} is not AI");
            return false;
        }
    }


    IEnumerator HandleEstate(Player player,estateBoard eBoard){
        Debug.Log(eBoard.owner.GetName());
        
                    if (eBoard.owner == bank 
                    
                    )
                    {
                        if(currentPlayer.playerData.circle>1){
                        if(player.playerData.isAI){
                        if(player.playerData.money>=eBoard.price&&AIBuyProperty(player,eBoard.price)){
                                 gameBehaviour.PayMoney(player,eBoard.price);
                                 gameBehaviour.AddProperty(player,eBoard);  
                                 generator.updateTile(eBoard);                          
                        }else{
                            Debug.Log($"地产 {eBoard.property} 开始拍卖");
                            isAuction=true;
                            

                             StartCoroutine(auction(eBoard));
                             yield return new WaitUntil(()=>!isAuction);
                             
                             isChecking = false;

                        }
                        

                    }
                    else{
               
                        bool? userChoice=null;
                        interactionPanel.ShowPanel($"are you want to buy {eBoard.property}?",eBoard.group,eBoard.price,eBoard.rent,(bool isBuy)=> 
                        { userChoice=isBuy;});
                        yield return new WaitUntil(()=>userChoice.HasValue);
                            if(userChoice.HasValue && userChoice.Value){
                               
                            if(player.playerData.money>eBoard.price){

                                 gameBehaviour.PayMoney(player,eBoard.price);
                                 gameBehaviour.AddProperty(player,eBoard);
                                 generator.updateTile(eBoard);
                                 
                            }else{
                                //此处执行没钱提示
                                Debug.Log("余额不足，请联系游戏管理员以获得充值方法");
                            }

                                }
                        else{//此处执行拍卖

                        Debug.Log($"地产 {eBoard.property} 开始拍卖");
                            isAuction=true;

                             StartCoroutine(auction(eBoard));
                             yield return new WaitUntil(()=>!isAuction);
                             
                             isChecking = false;
                             
                        }
                    }
                        isChecking = false;
                        yield break;
                        }
                            

                        
                        
                    }else if(eBoard.owner.GetName()==player.playerData.name){
                        Debug.Log("调用建筑脚本");
                        if(PlayerOwnsFullSet(player,eBoard)){
                            buildingButton.gameObject.SetActive(true);
                            
                            

                        }else{
        Debug.Log($"{player.name} cannot build on {eBoard.property} because they do not own all properties in this color set!");
       }
                        

                    }

                    else
                    {
                        gameBehaviour.PayRent(player, eBoard);
                        }
    }


        IEnumerator HadleBuyable(Player player,BuyableBoard bBoard){
        Debug.Log(bBoard.owner.GetName());
        
                    if (bBoard.owner == bank 
                    
                    )
                    {
                        if(currentPlayer.playerData.circle>1){

                        if(player.playerData.isAI){
                        if(player.playerData.money>=bBoard.price&&AIBuyProperty(player,bBoard.price)){
                                 gameBehaviour.PayMoney(player,bBoard.price);
                                 gameBehaviour.AddBuyable(player,bBoard);   
                                 generator.updateTile(bBoard);}
                           else{
                            Debug.Log($"地产 {bBoard.property} 开始拍卖");
                            isAuction=true;

                             StartCoroutine(BuyableAuction(bBoard));
                             yield return new WaitUntil(()=>!isAuction);
                             
                             isChecking = false;

                        }

                    }
                    else{
                        bool? userChoice=null;
                        interactionPanel.ShowPanel($"are you want to buy {bBoard.property}?",bBoard.group,bBoard.price,bBoard.rent,(bool isBuy)=> 
                        { userChoice=isBuy;});
                        yield return new WaitUntil(()=>userChoice.HasValue);
                            if(userChoice.HasValue && userChoice.Value){
                               
                            if(player.playerData.money>bBoard.price){

                                 gameBehaviour.PayMoney(player,bBoard.price);
                                 gameBehaviour.AddBuyable(player,bBoard);
                                 generator.updateTile(bBoard);
                            }else{
                                //此处执行没钱提示
                                Debug.Log("余额不足，请联系游戏管理员以获得充值方法");
                            }

                                }
                        else{//此处执行拍卖

                        Debug.Log($"地产 {bBoard.property} 开始拍卖");
                            isAuction=true;

                             StartCoroutine(BuyableAuction(bBoard));
                             yield return new WaitUntil(()=>!isAuction);
                             
                             isChecking = false;
                             
                        }
                    }
                        isChecking = false;
                        yield break;
                            

                        
                        
                    }}

                    else
                    {
                        gameBehaviour.PayBuyableRent(player, bBoard);
                    }
    }

    IEnumerator CheatingUppdate(){
        if(testmenus.isCheating){
        playerUpdate(currentPlayer);
        yield return null;
        }
    }
private bool PlayerOwnsFullSet(Player player, estateBoard board)
{
    Debug.Log("同色套装判断");
    foreach (Board b in RunGame.mapList)
    {estateBoard i = b as estateBoard;
    if(i!=null){
        if (i.group == board.group && i.owner.GetName() != player.playerData.name)
        {
            Debug.Log($"同色判断失败，对比组owner为 {board.owner.GetName()} , 失败者为{i.owner.GetName()} ");
            return false;
 
        }
       
    }else{
        continue;
    }
    }
    
    return true;
}
private void build(){
        estateBoard eBoard=mapList[currentPlayer.playerData.positionNo] as estateBoard;
    StartCoroutine(gameBehaviour.BuildBuilding(currentPlayer,eBoard));

    buildingButton.gameObject.SetActive(false);
}
public void cheatRoll(int i){
    cheatStep=i;

}
private void changeCameraMode(){
    if(cameraController==null){
        Debug.Log("camera controller does not exist");
    }else{
        if(cameraController.canBeDragging==false){
            cameraController.setInitialPosition();
            cameraController.canBeDragging=true;
            viewButton.GetComponent<Image>().color = new Color(187f/255f, 136f/255f, 54f/255f, 1f);
            
        }else{
            cameraController.canBeDragging=false;
            cameraController.resetPosition();
            viewButton.GetComponent<Image>().color = new Color(220f/255f, 250f/255f, 247f/255f, 1f);
            
        }
    }
}
private void showMenus(){
    menus.gameObject.SetActive(true);

}
private void closeMenus(){
    menus.gameObject.SetActive(false);

}
private  void quitGame(){
     #if UNITY_EDITOR
         UnityEditor.EditorApplication.isPlaying=false;
     #else
        Application.quit();
     #endif
    }

public void AutoSaveGame()
{
    
    cachedSaveData = new SaveData();
    cachedSaveData.allPlayers = new List<PlayerData>();
    foreach (Player p in playersList)
    {

        PlayerData copy = p.playerData.DeepCopy(); 
        foreach (Board b in copy.assetsList)
        {
            copy.assetsName.Add(b.property);
        }
        cachedSaveData.allPlayers.Add(copy); 
        Debug.Log($"自动缓存：{copy.name}, 在{copy.positionNo}号格子，有{copy.money}钱");
    }

    cachedSaveData.allBoards = new List<Board>(mapList);


    cachedSaveData.lCards = new List<Card>(luckCards);
    cachedSaveData.oCards = new List<Card>(opportunityCards);
    cachedSaveData.cPoint = point;
    cachedSaveData.freeParkingMoney = freeParkMoney;
    cachedSaveData.luckNo = luckNo;
    cachedSaveData.OpportunityNo = OpportunityNo;
    cachedSaveData.isai = isAI;
    cachedSaveData.diff = difficulty;

   
    }

public void SaveGame(){
     if (cachedSaveData == null)
    {
        Debug.LogWarning("还没有缓存的自动存档，无法手动保存！");
        return;
    }

    string json = JsonConvert.SerializeObject(cachedSaveData, Formatting.Indented, new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto
    });

    string path = Application.dataPath + "/save/savegame.json";
    System.IO.File.WriteAllText(path, json);

    Debug.Log("游戏已保存到：" + path);

}
}






[System.Serializable]
public class SaveData
{
    public List<PlayerData> allPlayers;

    public List<Board> allBoards;
  

    public int cPoint;

    public int freeParkingMoney;
    public int luckNo;
    public int OpportunityNo;
    public List<Card> lCards;
    public List<Card> oCards;
    public bool isai;
    public int diff;
}
