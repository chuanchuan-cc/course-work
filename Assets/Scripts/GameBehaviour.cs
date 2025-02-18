using UnityEngine;
using System.Collections.Generic;


public class GameBehaviour: MonoBehaviour
{
    public Player bank;
 

    public List<Board> mapList;
    public void AddMoney(Player player,int amount)
    {
        
        player.money += amount;
        Debug.Log($"{player.name} recieved £{amount}, new balance: £{player.money}");
        player.assetsWorth+=amount;
    }
    public void PayMoney(Player player,int amount)
    {
        if (player.isBankrupt)return;
        if (player.money >= amount)
        {
            player.money -= amount;
            Debug.Log($"{player.name} paid £{amount}, remaining balance: £{player.money}");
        player.assetsWorth-=amount;
        }
        
    }
    private void HandleBankruptcy(Player player)
    {
        player.isBankrupt = true;
        player.money = 0;
        player.assetsWorth = 0;
        foreach (estateBoard board in mapList){
            if(board.owner==player){
                board.owner=bank;
            }

        }
        player.assetsList.Clear();
        Debug.Log($"{player.name} is bankrupt! All assets are repossessed!");
    }
    public void GoToJail(Player player)
    {
        
        player.position = mapList.FindIndex(board=>board.group == "Go to jail");
        FreezeTurn(player, 2);

        
        
      Debug.Log($"{player.name} is sent to jail!");
    }
    public void FreezeTurn(Player player, int turns){
        player.freezeTurn = turns;
    }
  
    
    public void BuyProperty(Player player,estateBoard board)
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
        public void AddProperty(Player player, estateBoard board)
        {
            board.owner = player;
            player.assetsWorth+=board.price;
            player.assetsList.Add(board.property);

        }
       
   
        public void PayRent(Player player, estateBoard board)
        {
            if(!player.isBankrupt)
            {
                PayMoney(player, board.rent);       
                AddMoney(board.owner,board.rent);
                string _owner=board.owner.name;
                    
                
                Debug.Log($"{player.name} paid £{board.rent} in rent to {_owner}!");
            }
        }
     
    }

