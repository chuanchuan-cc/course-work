using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RunGame : MonoBehaviour
{
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
    private GameBehaviour gameBehaviour;
    //测试用玩家
    public Player testPlayer;
    int roll;
    void Awake(){
     
        DiceButton.interactable = false;
        gameBehaviour = BehavioursPool.GetComponent<GameBehaviour>();
        int playerNumber = PlayerPrefs.GetInt("PlayerNumber", 1);
        bool isAI = PlayerPrefs.GetInt("IsAI", 0) == 1;     
        point = 0;
        DiceButton.onClick.AddListener(ThrowDice);
        testPlayer.InitializePlayer("test");
        playersList.Add(testPlayer);
        //初始化version1 测试地图
        for(int i =0;i<40;i++)
        {
            mapList.Add(new Board("test",false));
        }
    }

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    string cardPath=PlayerPrefs.GetString("cardPath");
    (List<Card> luckCards,List<Card> opportunityCards)=CardLoader.LoadCards(cardPath);
 

    StartCoroutine(GameLoop());
   
    

}

IEnumerator GameLoop()
{
    
    
    while (keepGame)
    {
        DiceButton.interactable = false;
        //ui显示 玩家xxx的回合
        isEffectiveDice=false;
        Player currentPlayer = playersList[point];
        //显示玩家
        Debug.Log("当前玩家为"+currentPlayer.playerData.name);
        //
        int currentPoint = point;
        point = (point + 1) % playersList.Count;
        
        if (currentPlayer.playerData.freezeTurn > 0)
        {
            currentPlayer.playerData.freezeTurn -= 1;
            yield return new WaitForSeconds(1f);
            continue;
        }
        DiceButton.interactable = true;
       
        if(roll==-1){
            gameBehaviour.GoToJail(currentPlayer);
            yield return new WaitForSeconds(1f);
            continue;
        }

        yield return new WaitUntil(() => isEffectiveDice);

        if (!currentPlayer.isMoving) 
        {
            currentPlayer.Move(roll);
        }

        yield return new WaitUntil(() => !currentPlayer.isMoving); 
        
        
  
       

        if ((currentPlayer.playerData.positionNo + roll) > mapList.Count)
        {
            
               // gameBehaviour.AddMoney(currentPlayer, 200);
                //gameBehaviour.PayMoney(bank, 200);
            
            currentPlayer.playerData.circle += 1;
        }
    


        //Check(currentPlayer, mapList);
        //check包含行为判断加执行

        /*
        此处为停止条件
        if (currentPlayer.playerData.isBankrupt)
        {
            playersList.RemoveAt(currentPoint);
        }

        if (playersList.Count == 1)
        {
            break;
        }
        */

        yield return new WaitForSeconds(1f);
    }
}

public void ThrowDice(){
    int roll1;
    int roll2;
    int t=0;
    do{
        if(t>=3){
            roll=-1;
            isEffectiveDice = false;
            break;
        }
        roll1=Random.Range(1,7);
        roll2=Random.Range(1,7);
        t+=1;
        roll=roll1+roll2;
    }while(roll%2==0 && t<3);
    isEffectiveDice = true;
    


}


  
}
