using UnityEngine;
using System.Collections.Generic;

public class RunGame : MonoBehaviour
{
    public Player bank;
    public List<Player> playersList;
    public List<Board> mapList;

    public bool keepGame=true;
    public int point;

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   void Start()
{
    StartCoroutine(GameLoop());
}

IEnumerator GameLoop()
{
    int playerNumber = GameManager.playerNumberToGame;
    bool isAI = GameManager.isAIToGame;

    point = 0;

    while (keepGame)
    {
        Player currentPlayer = playersList[point];
        int currentPoint = point;
        point = (point + 1) % playersList.Count;

        if (currentPlayer.freezeTurn > 0)
        {
            currentPlayer.freezeTurn -= 1;
            continue;
        }

        int roll = ThrowDice();
        
        if ((currentPlayer.position + roll) > mapList.Count)
        {
            if (currentPlayer.circle == 0)
            {
                AddMoney(currentPlayer, 200);
                PayMoney(bank, 200);
            }
            currentPlayer.circle += 1;
        }

        currentPlayer.position = (currentPlayer.position + roll) % mapList.Count;


        Check(currentPlayer, mapList);
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


  
}
