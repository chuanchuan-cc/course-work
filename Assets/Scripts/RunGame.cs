using UnityEngine;
using System.Collections.Generic;

public class Main : MonoBehaviour
{
    public List<Player> playersList;
    public List<Board> mapList;

    public bool keepGame=true;
    public int point;

     
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {


        //给maplist,playerslist赋值


       
       point=0;

        while(keepGame){
            Player currentPlayer=playersList[point];

              int currentPoint=point;
              point=(point+1)%(playersList.Count);

              if(currentPlayer.isFreezed){
                if(currentPlayer.freezeTurn==1){
                    currentPlayer.isFreezed=false;
                }
                else{      
                    currentPlayer.freezeTurn-=1;
                }
                continue;
                }

            int roll=ThrowDice();

            
            if((currentPlayer.position+roll)>mapList.Count){
               currentPlayer.circle+=1;
            }
            currentPlayer.position=(currentPlayer.position+roll)%(mapList.Count);
            Check(currentPlayer,mapList);
            if(currentPlayer.isBankrupt){
                playersList.RemoveAt(currentPoint);
            }



            if(playersList.Count==1){
                break;
            }
         

        }
    }

  
}
