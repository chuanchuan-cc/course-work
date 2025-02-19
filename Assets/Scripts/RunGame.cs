using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RunGame : MonoBehaviour
{
    public static Player bank;
    public List<Player> playersList;
    public List<Board> mapList;

    public bool keepGame=true;
    public int point;
    public Button DiceButton;
    public bool isEffectiveDice=false;
    public GameObject BehavioursPool;
    private GameBehaviour gameBehaviour;
    int roll;
    void Awake(){
        if(bank==null){
            bank= new Player("Bank");
            bank.money=50000;
        }
        DiceButton.interactable = false;
        gameBehaviour = BehavioursPool.GetComponent<GameBehaviour>();
        int playerNumber = PlayerPrefs.GetInt("PlayerNumber", 1);
        bool isAI = PlayerPrefs.GetInt("IsAI", 0) == 1;
        point = 0;
        DiceButton.onClick.AddListener(ThrowDice);
    }

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    StartCoroutine(GameLoop());
}

IEnumerator GameLoop()
{
    
    
    while (keepGame)
    {
        //ui显示 玩家xxx的回合
        isEffectiveDice = false;
        Player currentPlayer = playersList[point];
        int currentPoint = point;
        point = (point + 1) % playersList.Count;
        
        if (currentPlayer.freezeTurn > 0)
        {
            currentPlayer.freezeTurn -= 1;
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
        
  
       
        
        if ((currentPlayer.position + roll) > mapList.Count)
        {
            
                gameBehaviour.AddMoney(currentPlayer, 200);
                gameBehaviour.PayMoney(bank, 200);
            
            currentPlayer.circle += 1;
        }

        currentPlayer.position = (currentPlayer.position + roll) % mapList.Count;


        //Check(currentPlayer, mapList);
        //check包含行为判断加执行

        if (currentPlayer.isBankrupt)
        {
            playersList.RemoveAt(currentPoint);
        }

        if (playersList.Count == 1)
        {
            break;
        }

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
            break;
        }
        roll1=Random.Range(1,7);
        roll2=Random.Range(1,7);
        t+=1;
        roll=roll1+roll2;
    }while(roll%2==0 && t<3);
    if(roll%2==0){
        roll=-1;

    }
    isEffectiveDice = true;
    


}


  
}
