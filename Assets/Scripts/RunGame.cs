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
    public Player nextPlayer=null;
    public int point;

    

    public Slider musicSlider;
    public CGcontrol cGcontrol;
 


    private SaveData cachedSaveData; 
    




    int roll;

   void Awake()
{
    instance = this;
    isLoadGame = PlayerPrefs.GetInt("isLoadGame", 0) == 1;
        PlayerPrefs.SetInt("isLoadGame", 0);
    PlayerPrefs.Save();
    cGcontrol=FindObjectOfType<CGcontrol>();
   
        

        

        
        DiceButton.interactable = false;
        gameBehaviour = GameObject.Find("BehaviourPool").GetComponent<GameBehaviour>();
        cameraController= GameObject.Find("Main Camera").GetComponent<CameraController>();

        dashBoard = GameObject.Find("DashBoard");
        BoardConstructor=dashBoard.GetComponent<dashBoardConstructor>();




        
        playersPool = GameObject.Find("PlayersPool");
        


        
        
        bank=new Bank();

            bankpanel=GameObject.Find("bankPanel").GetComponent<bankPanel>();
        if(bankpanel==null){
            Debug.Log("bankpanel fail to initialize");
        }

        
        
       
       
        
       

        // find and bind CardUI
        cardUI = FindObjectOfType<CardUI>();

        //bind interaction panel

        interactionPanel=GameObject.Find("interactionPanel").GetComponent<playerInteractionPanel>();
        if(interactionPanel==null)Debug.Log("can't find interaction panel");
        else Debug.Log("already find interaction panel");
        
        //bind bank button
        BankButton=GameObject.Find("BankButton").GetComponent<Button>();


       



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
        Debug.Log("load game");
        string path = PlayerPrefs.GetString("savePath");
    if (!System.IO.File.Exists(path))
    {
        Debug.LogWarning("can not find the save file");
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
    point=saveData.cpoint;
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
                       

                    }else{
                        BuyableBoard bb= b as BuyableBoard;
                        if(bb!=null){
                            gameBehaviour.AddBuyable(player,bb);
                           
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
        point=0;
        



        //test
       



     
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


        //test
        for(int i=1;i<=playerNumber-1;i++){
            playersList[i].playerData.isAI=true;
        }


    
    
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
    
    musicSlider.onValueChanged.AddListener((value) => MusicController.Instance.setThemeVolume(value));

    bankpanel.ClosePanel();



    
    //start
    
    StartCoroutine(GameLoop());
    bankpanel.setmap(mapList);

    

   
    

}
void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.C))
        {
            testmenus.cheating(currentPlayer);
            Debug.Log($"{currentPlayer.name}, money={currentPlayer.playerData.money}, freejailNum= {currentPlayer.playerData.freeJail}, worth={currentPlayer.playerData.assetsWorth}"
       
       
       );
        }
        


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
        

        

        
        

       


        
        isEffectiveDice=false;
        if(nextPlayer==null)
        currentPlayer = playersList[point];
        else
        currentPlayer=nextPlayer;
        nextPlayer=playersList[(playersList.IndexOf(currentPlayer)+1)%playersList.Count];
        Debug.Log($"currently player is {currentPlayer.name}");
        AutoSaveGame();
        BoardConstructor.highlightPlayer(currentPlayer);

        PlayerDisplay playerDisplay=dashBoard.transform.Find(currentPlayer.name).GetComponent<PlayerDisplay>();


        Broadcast.showBroad(currentPlayer);
        yield return new WaitForSeconds(1f);
        Broadcast.closeBroad(currentPlayer);
        yield return new WaitUntil(()=>!Broadcast.isBroadcasting);
        BankButton.interactable=true;
        viewButton.interactable=true;
        menusButton.interactable=true;


   

        
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

        //test cheatting menu
        roll=(cheatStep!=0)? cheatStep: roll;
        oldPosNo=currentPlayer.playerData.positionNo;
        int PassGoNum=(roll>0)? roll:0;
        
        
        
        if(roll==-1){
            
            gameBehaviour.GoToJail(currentPlayer);
          
            
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
           
            bank2player(currentPlayer,200);

            
            currentPlayer.playerData.circle += 1;
        }
    


        StartCoroutine(check(currentPlayer));
      

        

      
        yield return new WaitUntil(()=>!isChecking);
        

        if (playersList.Count == 1)
        {
            win();
            
        }

        

        
        
        
        NextButton.interactable=true;

        yield return new WaitUntil(() => isNext||currentPlayer.playerData.isAI||currentPlayer.playerData.isBankrupt);

        

    }
}

public void win(){
    string winner=playersList[0].playerData.name;
    Broadcast.win(winner);
}
public void playerUpdate(Player p){
    Debug.Log($"update {p.name}'s dashboard");
    Transform tf = dashBoard.transform.Find(p.name);
    if(tf!=null){

    PlayerDisplay display = tf.GetComponent<PlayerDisplay>();
    if (display != null)
    {
        display.UpdateDisplay(p);
  
    }
    else
    {
        Debug.LogError($"can not find  {p.name}'s PlayerDisplay");
    }

}
}

public void deletePlayer(Player player){
    if(player==nextPlayer)
    nextPlayer=playersList[(playersList.IndexOf(nextPlayer)+1)%playersList.Count];
    playersList.Remove(player);
    Transform child=dashBoard.transform.Find(player.name);
    if (child!=null)
    {
        Destroy(child.gameObject);
    }
    BoardConstructor.RebuildLayout();
    isNext=true;
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
        int inimoney=player.playerData.money;
        
        
        Board currentBoard = mapList[player.playerData.positionNo];
      

        // is card?
        if (currentBoard.property == "Opportunity Knocks" || currentBoard.property == "Pot Luck")
        {
            Debug.Log("drawcard");
            
            StartCoroutine(DrawCard(player,currentBoard));


            yield return new WaitUntil(()=>!isProcessingCard);
            
            
            isChecking = false;
            yield break;
        }else{

        
        
            StartCoroutine(HandleBoard(player, currentBoard));
        }
        if(currentPlayer!=null)
        if(player.playerData.money>inimoney)
        cGcontrol.PlayCG("add_money",player);
        if(player.playerData.money<inimoney)
        cGcontrol.PlayCG("money_fly",player);

        
        playerUpdate(currentPlayer);
        yield return new WaitForSeconds(0.2f);
        isChecking=false;
        
    }


public IEnumerator HandleBoard(Player player, Board currentBoard){

    if(currentBoard.property=="Free Parking"){
            gameBehaviour.AddMoney(currentPlayer,freeParkMoney);
  
        }
        else if(currentBoard.property=="Go to Jail"){
            if (player.playerData.freeJail > 0)
        {
            player.playerData.freeJail--;
            player.directlyMove(mapList.Find(board=>board.property == "Jail/Just visiting"));

        }
        else{
            gameBehaviour.GoToJail(currentPlayer);
            }
      
        }
        else if(currentBoard.property=="Income Tax"){
            gameBehaviour.PayMoney(currentPlayer,200);

        }
        else if(currentBoard.property=="Super Tax"){
            gameBehaviour.PayMoney(currentPlayer,100);

        }
            else if (currentBoard.canBeBought)
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


    IEnumerator DrawCard(Player player,Board board)
    {
        isProcessingCard=true;
        bool isInteracting=false;
                   
  
        
        
        Card drawnCard;
        List<Card> deck;
        if(board.property=="Pot Luck"){
            deck = luckCards;
         drawnCard = deck[luckNo];
        Debug.Log($"{player.name} draw {drawnCard.description}");
        luckNo=(luckNo+1)%deck.Count;
        }
        else{
             deck =opportunityCards;
           drawnCard = deck[OpportunityNo];
        Debug.Log($"{player.name} draw {drawnCard.description}");
        OpportunityNo=(OpportunityNo+1)%deck.Count;  
        }

        // show card
        cardUI.ShowCard(drawnCard);
     
                  MusicController.Instance.PlayCardSound();
             
        
                float timer = 0f;
  
    while (timer < 5f)
    {
        if(currentPlayer.playerData.isAI){
            isProcessingCard=false;
            break;
        }
        if (Input.GetMouseButtonDown(0)) 
        {
            Debug.Log("click, close the ui");
            isProcessingCard=false;
            break;
        }
        timer += Time.deltaTime;
        yield return null;   
    }
        cardUI.HideCard();

        yield return new WaitUntil(()=>!cardUI.isDisplaying);


        if(drawnCard.isInteractable){
            isInteracting=true;


                 bool? userChoice=null;
                 string[] parts=drawnCard.description.Replace("\"","").Split(" or ");
               
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


        // handle card
        ApplyCardEffect(player, drawnCard);

        yield return new WaitUntil(()=>!isApplyCard);
        isProcessingCard=false;
        
    }


    void ApplyCardEffect(Player player, Card card)
    {
        isApplyCard=true;
       
        if (card.isMove)
        {
            if(card.steps!=0){
                Board target= mapList[(player.playerData.positionNo+card.steps)%40];
            
                player.directlyMove(target);
                if(player.playerData.positionNo<oldPosNo&&card.steps>0)
                    player.playerData.circle++;
                if(player.playerData.positionNo>oldPosNo&&card.steps<0)
                player.playerData.circle--;

                
            }
            else{
          
   
                foreach(Board i in mapList){
                    if(i.property.ToLower()==card.destinationName.ToLower()){
                        
                        player.directlyMove(i);
                        break;
                    }
               
                    

                      if(card.isFoward)
                {
                    
                    if(player.playerData.positionNo<oldPosNo){
                        player.playerData.circle++;
                        if(!card.collectGo)
                        bank2player(player,200);
                        
                        
                        
                    }

                }
                else
                {
             
                    if(player.playerData.positionNo>oldPosNo){
                        player.playerData.circle--;
                    }

                }
                 
                 

                    }}
                


                StartCoroutine(moveCheck(player));
                isApplyCard=false;
                 return;
                
            
            
        }
        
        if (card.isPay)
        {
            if(card.payee=="player"&&card.payer=="bank"){
               bank.money-=card.moneyAmount;
               gameBehaviour.AddMoney(player, card.moneyAmount);
               
            }

           
            else if(card.payee=="bank"&&card.payer=="player"){bank.money+=card.moneyAmount;
            gameBehaviour.PayMoney(player, card.moneyAmount);
            }
            else if(card.isPayByAll){
                foreach(Player p in playersList){
                    gameBehaviour.AddMoney(player,card.moneyAmount);
                    gameBehaviour.PayMoney(p,card.moneyAmount);
                }
            }
            
            
            
            }
        if(card.isPayFine){
       
            
            freeParkMoney+=card.moneyAmount;
          
            gameBehaviour.PayMoney(player,card.moneyAmount);

        }
        

        
        if (card.isJailFree)
        {
           
            player.playerData.freeJail += 1;
        }
        if (card.isGoJail)
        {
           
            gameBehaviour.GoToJail(player);
        }
        if(card.isRepair){
            int houseNum=0;
            int hotelNum=0;
            foreach(Board b in player.playerData.assetsList){
                estateBoard eb= b as estateBoard;
                if(eb!=null){
                    if(eb.improvedLevel>0&&eb.improvedLevel<5)
                    houseNum++;
                    if(eb.improvedLevel==5)
                    hotelNum++;

                }
            }
         
            gameBehaviour.PayMoney(player,card.houseRepair*houseNum+card.hotelRepair*hotelNum);
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
        foreach(Player p in playersList){
            
            //test circle
            // if(p.playerData.circle>=1) auctionList.Add(playersList[t]);

            
            if(p.playerData.circle>1 && p.name!=currentPlayer.name) auctionList.Add(p);
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

        public List<Player> getplayerlist(){
            return playersList;
        }
    IEnumerator BuyableAuction(BuyableBoard bBoard){
        isAuction=true;
        int auctionPrice=bBoard.price;
        int detlaPrice=auctionPrice/10;
        int totalNum=playersList.Count-1;
        List<Player> auctionList=new List<Player>();
        foreach(Player p in playersList){
            
            //test circle
            // if(p.playerData.circle>=1) auctionList.Add(playersList[t]);
       
            if(p.playerData.circle>1 && p.name!=currentPlayer.name) auctionList.Add(p);
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
    if(cameraController.canBeDragging){
        changeCameraMode();}
    StartCoroutine(showBankPanel());

}
private void bank2player(Player player, int i){
    if(i>0)
                   gameBehaviour.AddMoney(player, i);

            else gameBehaviour.PayMoney(player,-i);
               bank.money-=i;

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
                //hard
                return false;
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
                //hard
                return false;
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
                            Debug.Log($"{eBoard.property} start auction");
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
                                bankpanel.showLackOfCashPanel(player,eBoard.price);
                                yield return new WaitUntil(()=>!bankpanel.isLackCash);
                                if(player.playerData.money>eBoard.price){
                                    gameBehaviour.PayMoney(player,eBoard.price);
                                 gameBehaviour.AddProperty(player,eBoard);
                                 generator.updateTile(eBoard);

                                }
                             
                            }

                                }
                        else{//auction

                        Debug.Log($"{eBoard.property} actuion");
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
              
                        if(canBuild(player,eBoard)){
                            buildingButton.gameObject.SetActive(true);
                            
                            

                        }else{
        Debug.Log($"{player.name} cannot build on {eBoard.property} because they do not own all properties in this color set!");
       }
                        

                    }

                    else
                    {
                        gameBehaviour.PayRent(currentPlayer,eBoard);
                        
                        }
                        generator.updateTile(eBoard);
                        playerUpdate(player);
                        isChecking=false;
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
                            Debug.Log($"{bBoard.property} start auction");
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
                                bankpanel.showLackOfCashPanel(player,bBoard.price);
                                yield return new WaitUntil(()=>!bankpanel.isLackCash);
                                if(player.playerData.money>bBoard.price){
                                    gameBehaviour.PayMoney(player,bBoard.price);
                                 gameBehaviour.AddBuyable(player,bBoard);
                                 generator.updateTile(bBoard);

                                }
                
                            }

                                }
                        else{//auction

                        Debug.Log($"{bBoard.property} start auction");
                            isAuction=true;

                             StartCoroutine(BuyableAuction(bBoard));
                             yield return new WaitUntil(()=>!isAuction);
                             
                             isChecking = false;
                             
                        }
                    }
                    generator.updateTile(bBoard);
                        playerUpdate(player);
                        isChecking = false;
                        yield break;
                            

                        
                        
                    }}

                    else
                    {
                        gameBehaviour.PayBuyableRent(player, bBoard);
                    }
    }


private bool canBuild(Player player, estateBoard board)
{
    int minlevel=5;

    foreach (Board b in RunGame.mapList)
    {estateBoard i = b as estateBoard;
    if(i!=null){
        if (i.group == board.group)
        {
           
            minlevel=(minlevel>i.improvedLevel)?i.improvedLevel:minlevel;
            if(i.owner.GetName() != player.playerData.name)
            return false;

        }
       
    }else{
        
        continue;
    }
    }
    if (board.improvedLevel >= 5)
    return false;
    else if( board.improvedLevel<minlevel+1)
    return true;
    else
    return false;
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
    if(cameraController.canBeDragging){
        changeCameraMode();}
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
    private IEnumerator moveCheck(Player player){
        yield return new WaitUntil(()=>!player.isMoving);
        StartCoroutine(check(player));

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
    
    }

    cachedSaveData.allBoards = new List<Board>(mapList);


    cachedSaveData.lCards = new List<Card>(luckCards);
    cachedSaveData.oCards = new List<Card>(opportunityCards);
    cachedSaveData.cpoint = playersList.IndexOf(currentPlayer);
    cachedSaveData.freeParkingMoney = freeParkMoney;
    cachedSaveData.luckNo = luckNo;
    cachedSaveData.OpportunityNo = OpportunityNo;
    cachedSaveData.isai = isAI;
    cachedSaveData.diff = difficulty;

   
    }

public void SaveGame(){
     if (cachedSaveData == null)
    {
        Debug.LogWarning("no auto save");
        return;
    }

    string json = JsonConvert.SerializeObject(cachedSaveData, Formatting.Indented, new JsonSerializerSettings
    {
        TypeNameHandling = TypeNameHandling.Auto
    });

    string path = Application.dataPath + "/save/savegame.json";
    System.IO.File.WriteAllText(path, json);

    Debug.Log("game was saved in" + path);

}
public void cheat(){
    testmenus.setmapplayer(mapList,playersList);
}


}

[System.Serializable]
public class SaveData
{
    public List<PlayerData> allPlayers;

    public List<Board> allBoards;
  

    public int cpoint;

    public int freeParkingMoney;
    public int luckNo;
    public int OpportunityNo;
    public List<Card> lCards;
    public List<Card> oCards;
    public bool isai;
    public int diff;
}
