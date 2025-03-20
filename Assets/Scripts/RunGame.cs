using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;


public class RunGame : MonoBehaviour
{
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
    Player currentPlayer;
    public CGcontrol cgControl;

    public int point;
    public Button DiceButton;
    public Button NextButton;
    public bool isEffectiveDice = false;

    public GameObject playersPool;
    private GameBehaviour gameBehaviour;
    private Player lastPlayer = null;
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

    //测试用玩家

    int roll;

   void Awake()
{
    bool isLoadGame = PlayerPrefs.GetInt("IsLoadGame", 0) == 1;
    if (isLoadGame)
    {
        // 加载游戏的逻辑
    }
    else
    {   
        // read the input from last scene

        
        isAI = PlayerPrefs.GetInt("IsAI", 0) == 1;
        difficulty=PlayerPrefs.GetInt("difficulty",0);
        int playerNumber = PlayerPrefs.GetInt("PlayerNumber", 1);
        playerNumber=(isAI)? playerNumber+1:playerNumber;



        
        isAI=false;
        difficulty=0;
        playerNumber=2;
        

        //initialize players
        point = 0;

        
        DiceButton.interactable = false;
        gameBehaviour = GameObject.Find("BehaviourPool").GetComponent<GameBehaviour>();

        dashBoard = GameObject.Find("DashBoard");
        BoardConstructor=dashBoard.GetComponent<dashBoardConstructor>();




        
        playersPool = GameObject.Find("PlayersPool");
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
                
                
   
        
            player.InitializePlayer(player.gameObject.name,0);
            
            playersList.Add(player);
            }
            }}
        

        if(isAI) playersList[playerNumber-1].playerData.isAI=true;
        
        
        bank=new Bank();

            bankpanel=GameObject.Find("bankPanel").GetComponent<bankPanel>();
        if(bankpanel==null){
            Debug.Log("bankpanel初始化失败");
        }

        
        
       
       
        
        foreach(Player player in playersList){
            BoardConstructor.CreateChildren(player);
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

        

    }
    NextButton.gameObject.SetActive(false);

}

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    
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
    if(mapPath== null){
        Debug.LogError("cardPath is null");
    }else{
    try{(luckCards,opportunityCards)=CardLoader.LoadCards(cardPath);
     
    Shuffle(luckCards);
    Shuffle(opportunityCards);
    }catch(System.Exception e){
        Debug.LogError("Error happen when initialize the card:"+e.Message);
    }

    }

    foreach( Board board in mapList){
        string mapkey=(board.group==null)? board.group.ToLower():board.property.ToLower();
        

    }
    generator=GameObject.Find("Map").GetComponent<TileGenerator>();
    generator.GenerateMapFromList(mapList);
    BankButton.onClick.AddListener(showBankPanel);
    BankButton.interactable=false;

    bankpanel.ClosePanel();



    
    //start
    
    StartCoroutine(GameLoop());
   
    

}




    IEnumerator GameLoop()
{


    
    diceRolls=0;
    
    while (keepGame)
    {
        
        cardUI.HideCard();
        isbehavior=false;
        isNext=false;
        DiceButton.gameObject.SetActive(false);
        NextButton.gameObject.SetActive(true);
        NextButton.interactable=false;
        

       


        
        isEffectiveDice=false;
        
        currentPlayer = playersList[point];
        PlayerDisplay playerDisplay=dashBoard.transform.Find(currentPlayer.name).GetComponent<PlayerDisplay>();


        Broadcast.showBroad(currentPlayer);
        yield return new WaitForSeconds(1f);
        Broadcast.closeBroad(currentPlayer);
        yield return new WaitUntil(()=>!Broadcast.isBroadcasting);
        BankButton.interactable=true;


        int currentPoint = point;
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
        
        
  
       


        if ((currentPlayer.playerData.positionNo + roll) > mapList.Count)
        {
            
               gameBehaviour.AddMoney(currentPlayer, 200);
               bank.money-=200;
            
            currentPlayer.playerData.circle += 1;
        }
    


        StartCoroutine(check(currentPlayer));
      

        /*
        此处为停止条件
        if (currentPlayer.isBankrupt)
        {
            playersList.RemoveAt(currentPoint);
        }

        if (playersList.Count == 1)
        {
            break;
        }
        */
        yield return new WaitUntil(()=>!isChecking);
        
        foreach (Player p in playersList)
{
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

        
        
        lastPlayer=currentPlayer;
        NextButton.interactable=true;

        yield return new WaitUntil(() => isNext);
        

    }
}


public void ThrowDice()
{
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
    isbehavior=true;
    DiceButton.interactable = false;
    int roll1, roll2,roll;
    diceRolls =0;
    while(diceRolls<3){
        diceRolls+=1;
        roll1 = Random.Range(1, 7);
        roll2 = Random.Range(1, 7);
        roll = roll1 + roll2;
       
    if(roll%2!=0){
        isbehavior=false;
        isEffectiveDice = true;
        return;
        
    }    
    roll=-1;
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
                {Debug.Log(eBoard.owner.GetName());
                    if (eBoard.owner == bank)
                    {
                        bool? userChoice=null;
                        interactionPanel.ShowPanel($"are you want to buy {eBoard.property}?",eBoard.group,eBoard.price,eBoard.rent,(bool isBuy)=> 
                        { userChoice=isBuy;});
                        yield return new WaitUntil(()=>userChoice.HasValue);
                            if(userChoice.HasValue && userChoice.Value){
                               
                            if(player.playerData.money>eBoard.price){
                                //gameBehaviour.BuyProperty(player,eBoard);
                                 gameBehaviour.PayMoney(player,eBoard.price);
                                 gameBehaviour.AddProperty(player,eBoard);
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
                        isChecking = false;
                        yield break;
                            

                        
                        
                    }

                    else
                    {
                        gameBehaviour.PayRent(player, eBoard);
                    }
                }
            }
        }
        isChecking=false;
    }


    IEnumerator DrawCard(Player player,Board board)
    {
        isProcessingCard=true;
        
        
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


        // 处理卡片效果
        ApplyCardEffect(player, drawnCard);
        float timer = 0f;
        Player initialPlayer = currentPlayer; 
    while (timer < 5f||isApplyCard)
    {
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
        isProcessingCard=false;
        
    }

    void ApplyCardEffect(Player player, Card card)
    {
        isApplyCard=true;
        if (card.isMove)
        {
            int t;
                foreach(Board i in mapList){
                    if(i.property==card.destinationName){
                        t=i.positionNo-player.playerData.positionNo;
                      if(card.isFoward)
                {t=(t>0)?t:40+t;}
                else
                {t=(t<0)?t:40-t;}
                 player.Move(t);
                 isApplyCard=false;
                 return;

                    }
                }
                
            
            
        }
        if (card.isPay)
        {
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
            freeParkMoney+=card.moneyAmount;

        }
        
        if (card.isJailFree)
        {
            player.playerData.freeJail += 1;
        }
        if (card.isGoJail)
        {
            gameBehaviour.GoToJail(player);
        }
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
            // if(p.playerData.circle>0) auctionList.Add(playersList[t]);
            if(p.playerData.circle>=0) auctionList.Add(playersList[t]);
            else continue;
        }
        while(auctionList.Count>0){
            Player buyer=null;
            int p=auctionList.Count;
            for(int i =0; i<p;i++){
                Player acutionPlayer= auctionList[i];
            bool? userChoice=null;
                        interactionPanel.ShowPanel($"{acutionPlayer.name}, the auction price of {eBoard.property} is {auctionPrice}, are you want to buy this estate?",eBoard.group,eBoard.price,eBoard.rent,(bool isBuy)=> 
                        { userChoice=isBuy;
                      ;
                        });
                        yield return new WaitUntil(()=>userChoice.HasValue);
                            if(userChoice.HasValue && userChoice.Value){
                                if(acutionPlayer.playerData.money<=eBoard.price){
                                    //此处加拍卖没钱ui
                                    Debug.Log("拍卖没钱");
                                    auctionList.RemoveAt(i);
                                    continue;
                                }
                                else{ auctionPrice+=detlaPrice;
                                buyer=acutionPlayer;
                                }
                               
                            }else
                                auctionList.RemoveAt(i);
                                
                                
                            
                            
                            }
                            if(auctionList.Count==1||buyer!=null){
            gameBehaviour.PayMoney(buyer,eBoard.price);
            gameBehaviour.AddProperty(buyer,eBoard);
            isAuction=false;
            yield break;

            }
            if(auctionList.Count==0||buyer==null){
                isAuction=false;
                yield break;
            }
      

        }
        
        
        

        

    }
    private void showBankPanel(){
        bankpanel.ShowPanel();
        
    }
   
}
