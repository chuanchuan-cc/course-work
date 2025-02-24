using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RunGame : MonoBehaviour
{
    public bool isLoadGame;
    public static Player bank;
    public List<Player> playersList= new List<Player>();
    public List<Board> mapList;
    public List<Card> luckCards;
    public List<Card> opportunityCards;

    public bool keepGame=true;
    public int point;
    public Button DiceButton;
    public bool isEffectiveDice=false;
    public GameObject BehavioursPool;
    public GameObject playersPool;
    private GameBehaviour gameBehaviour;
    private Player lastPlayer=null;
    private int diceRolls;
    //测试用玩家

    int roll;
    void Awake(){
        bool isLoadGame = PlayerPrefs.GetInt("IsLoadGame", 0) == 1; 
        //非load方法，新游戏方法。
        if(isLoadGame){

        }
        else{
        DiceButton.interactable = false;
        gameBehaviour = BehavioursPool.GetComponent<GameBehaviour>();
        playersPool=GameObject.Find("PlayersPool");
        int playerNumber = PlayerPrefs.GetInt("PlayerNumber", 1);
        bool isAI = PlayerPrefs.GetInt("IsAI", 0) == 1;     
        point = 0;
        DiceButton.onClick.AddListener(ThrowDice);
        playersPool=GameObject.Find("PlayersPool");
        foreach(Transform child in playersPool.transform){
            Player player=child.GetComponent<Player>();
            player.InitializePlayer(player.gameObject.name);
            playersList.Add(player);
            if (player != null)
    {
        Debug.Log("find Player:" + child.name);
    }
        }
        
        //初始化version1 测试地图
        for(int i =0;i<40;i++)
        {
            mapList.Add(new Board(i,"test","test","used for test",false));
        }
        }
    }

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    
    string cardPath=PlayerPrefs.GetString("cardPath");
    (List<Card> luckCards,List<Card> opportunityCards)=CardLoader.LoadCards(cardPath);
    //测试卡组
    foreach(Card card in luckCards){
        Debug.Log($"card{card.description} load successful");
    }
    foreach(Card card in opportunityCards){
        Debug.Log($"card{card.description} load successful");
    }

    StartCoroutine(GameLoop());
   
    

}

IEnumerator GameLoop()
{
    diceRolls=0;
    
    while (keepGame)
    {
        DiceButton.interactable = false;
        //ui显示 玩家xxx的回合
        isEffectiveDice=false;
        
        Player currentPlayer = playersList[point];
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
        lastPlayer=currentPlayer;
        yield return new WaitForSeconds(0.2f);
    }
}

public void ThrowDice()
{
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
 

void check(Player player){
Board currentBoard=mapList[player.playerData.positionNo];
if(player.playerData.freeJail>0){
player.playerData.freeJail--;
}
else{
    if(currentBoard.canBeBought){
        estateBoard eBoard= currentBoard as estateBoard;
        if(eBoard!=null){
            if(eBoard.owner==bank){
            //此处写是否购买资产方法
            if(eBoard.owner!=player){
               gameBehaviour.PayRent(player,eBoard);

            }
        }
   

  
       }
     }
  }
}
}

