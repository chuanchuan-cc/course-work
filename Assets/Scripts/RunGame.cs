using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RunGame : MonoBehaviour
{
    public bool isLoadGame;
    public static Bank bank;
    public static List<Player> playersList = new List<Player>();
    public static List<Board> mapList;
    public static List<Card> luckCards;
    public static List<Card> opportunityCards;
    public int luckNo=0;
    public int OpportunityNo=0;

    public bool keepGame = true;
    Player currentPlayer;


    public int point;
    public Button DiceButton;
    public bool isEffectiveDice = false;
    public GameObject BehavioursPool;
    public GameObject playersPool;
    private GameBehaviour gameBehaviour;
    private Player lastPlayer = null;
    private int diceRolls;
    public CardUI cardUI;  // 新增：控制卡片 UI
    public static bool setFollow=false;

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
        DiceButton.interactable = false;
        gameBehaviour = BehavioursPool.GetComponent<GameBehaviour>();
        playersPool = GameObject.Find("PlayersPool");

        // 自动查找并绑定 CardUI
        cardUI = FindObjectOfType<CardUI>();
    

        int playerNumber = PlayerPrefs.GetInt("PlayerNumber", 1);
        bool isAI = PlayerPrefs.GetInt("IsAI", 0) == 1;
        point = 0;
        DiceButton.onClick.AddListener(ThrowDice);

        foreach (Transform child in playersPool.transform)
        {
            Player player = child.GetComponent<Player>();
            player.InitializePlayer(player.gameObject.name);
            playersList.Add(player);
            if (player != null)
            {
                Debug.Log("find Player: " + child.name);
    {
        Debug.Log("find Player:" + child.name);
    }
        }
        
        bank=new Bank();
 
       
        }
    }
}

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    
    //测试直接写入测试地图位置
    //string cardPath=PlayerPrefs.GetString("cardPath");
    //string mapPath=PlayerPrefs.GetString("mapPath");
    //
    string mapPath=Application.dataPath+"/Resources/map/testMap.xlsx";
    string cardPath=Application.dataPath+"/Resources/card/testCard.xlsx";
    //以上为直接写入的地址
    //
    Debug.Log(mapPath);
    mapList=BoardLoader.LoadBoards(mapPath);
    (luckCards,opportunityCards)=CardLoader.LoadCards(cardPath);
    foreach(Board board in mapList){
        Debug.Log($"Board No.{board.positionNo}, {board.property} load successful");
    }
    //测试卡组
    foreach(Card card in luckCards){
        Debug.Log($"card {card.description} load successful");
    }
    foreach(Card card in opportunityCards){
        Debug.Log($"card {card.description} load successful");
    }
        


    //start
    
    StartCoroutine(GameLoop());
   
    

}




    IEnumerator GameLoop()
{

    
    diceRolls=0;
    
    while (keepGame)
    {
        cardUI.HideCard();
        

        setFollow=false;
       

        DiceButton.interactable = false;
        //ui显示 玩家xxx的回合
        isEffectiveDice=false;
        
        currentPlayer = playersList[point];

        int currentPoint = point;
        point = (point + 1) % playersList.Count;
        
        if (currentPlayer.playerData.freezeTurn > 0)
        {
            currentPlayer.playerData.freezeTurn -= 1;
            yield return new WaitForSeconds(0.5f);
            continue;
        }
        if(lastPlayer==null||!lastPlayer.isMoving)DiceButton.interactable = true;
        
        yield return new WaitUntil(() => isEffectiveDice);
        if(roll==-1){
            gameBehaviour.GoToJail(currentPlayer);
            yield return new WaitForSeconds(0.5f);
            continue;
        }else if (!currentPlayer.isMoving) 
        {
            currentPlayer.Move(roll);
            setFollow=true;



        }

        yield return new WaitUntil(() => !currentPlayer.isMoving); 
        
        DiceButton.interactable = false;
  
       

        if ((currentPlayer.playerData.positionNo + roll) > mapList.Count)
        {
            
               // gameBehaviour.AddMoney(currentPlayer, 200);
                //gameBehaviour.PayMoney(bank, 200);
            
            currentPlayer.playerData.circle += 1;
        }
    


        check(currentPlayer);
      

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
        currentPlayer.UpdateUI();
        lastPlayer=currentPlayer;

        yield return new WaitUntil(() => !cardUI.isDisplaying);

    }
}


public void ThrowDice()
{
    DiceButton.interactable = false;
    int roll1, roll2;

    roll1 = Random.Range(1, 7);
    roll2 = Random.Range(1, 7);
    //roll = roll1 + roll2;
    roll=7;
    Debug.Log($"roll1={roll1}, roll2={roll2}, roll={roll}, diceRolls={diceRolls}");
    if (roll % 2 == 0)
    {
        diceRolls++;
        if (diceRolls == 3)
        {
            roll = -1;
            diceRolls=0;
            isEffectiveDice = true;
            return;
        }else
        {
            DiceButton.interactable = true;
            return;
        }
    }else
    {
        isEffectiveDice = true;
        diceRolls=0;
           
    }

       
   

}
 

 void check(Player player)
    {
        Board currentBoard = mapList[player.playerData.positionNo];
      

        // 判断是否踩到抽卡格子
        if (currentBoard.property == "Opportunity Knocks" || currentBoard.property == "Pot Luck")
        {
            Debug.Log("触发drawcard");
            StartCoroutine(DrawCard(player,currentBoard));
            
            return;
        }

        if (player.playerData.freeJail > 0)
        {
            player.playerData.freeJail--;
        }
        else
        {
            if (currentBoard.canBeBought)
            {
                estateBoard eBoard = currentBoard as estateBoard;
                if (eBoard != null)
                {
                    if (eBoard.owner == bank)
                    {
                        // 玩家可以购买资产
                    }
                    else if (eBoard.owner != player)
                    {
                        gameBehaviour.PayRent(player, eBoard);
                    }
                }
            }
        }
    }


    IEnumerator DrawCard(Player player,Board board)
    {
        
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
    while (timer < 5f)
    {
        if (Input.GetMouseButtonDown(0)) // 监听鼠标点击
        {
            Debug.Log("点击屏幕，立即关闭卡片 UI");
            break;
        }
        timer += Time.deltaTime;
        yield return null;   
    }
        cardUI.HideCard();
        
    }

    void ApplyCardEffect(Player player, Card card)
    {
        if (card.isMove)
        {
            int steps = card.isFoward ? card.moneyAmount : -card.moneyAmount;
            player.Move(steps);
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
            
        
        if (card.isJailFree)
        {
            player.playerData.freeJail += 1;
        }
        if (card.isGoJail)
        {
            gameBehaviour.GoToJail(player);
        }
    }
}
