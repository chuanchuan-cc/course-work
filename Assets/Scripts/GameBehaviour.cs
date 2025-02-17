using UnityEngine;
using System.Collections.Generic;


public class gameBehaviour: MonoBehaviour
{
 

    public List<Board> mapList;
    public void AddMoney(Player player,int amount)
    {
        
        player.money += amount;
        Debug.Log($"{player.name} recieved £{amount}, new balance: £{player.money}");
        player.assetsValue+=amount;
    }
    public void PayMoney(Player player,int amount)
    {
        if (isBankrupt)return;
        if (player.money >= amount)
        {
            player.money -= amount;
            Debug.Log($"{player.name} paid £{amount}, remaining balance: £{player.money}");
        player.assetsValue-=amount;
        }
        
    }
    private void HandleBankruptcy(Player player)
    {
        player.isBankrupt = true;
        player.money = 0;
        Debug.Log($"{player.name} is bankrupt! All assets are repossessed!");
    }
    public void GoToJail(Player player)
    {
        foreach(var board in mapList){
            if (board.group == "Go to jail"){
                player.position = board.position;
                FreezeTurn(player, 2);

        }
        }
      Debug.Log($"{player.name} is sent to jail!");
    }
    public void FreezeTurn(Player player, int i){
        player.freezeTurn = i;
        player.isFreezed = true;
    }
  
    
    public void BuyProperty(Player player,Board board)
    {

        if (player.money >= board.price)
        {
            PayMoney(player,board.price);
            AddProperty(player,board);
            Debug.Log($"{player.name} bought {board.property}!");
        }
        else
        {
            Debug.Log($"{player.name} does not have enough money to buy {board.property}!");

        }}
        public void AddProperty(Player player, Board board)
        {
            board.owner = player;
            player.assetsValue+=board.price;

        }
       
   
        public void PayRent(Player player, Board board)
        {
            if(!player.isBankrupt)
            {
                PayMoney(player, board.rent);       
                AddMoney(board.owner,board.rent);
                string _owner=board.owner;
                    
                
                Debug.Log($"{player.name} paid £{rentAmount} in rent to {_owner}!");
            }
        }
     
    }

