using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class GameBehaviour: MonoBehaviour
{
    public Player bank;
 

    public List<Board> mapList;
    public void AddMoney(Player player,int amount)
    {
        
        player.playerData.money += amount;
        Debug.Log($"{player.playerData.name} recieved £{amount}, new balance: £{player.playerData.money}");
        player.playerData.assetsWorth+=amount;
    }
    public void PayMoney(Player player,int amount)
    {
        if (player.playerData.isBankrupt)return;
        if (player.playerData.money >= amount)
        {
            player.playerData.money -= amount;
            Debug.Log($"{player.playerData.name} paid £{amount}, remaining balance: £{player.playerData.money}");
        player.playerData.assetsWorth-=amount;
        }
        
    }
    private void HandleBankruptcy(Player player)
    {
        player.playerData.isBankrupt = true;
        player.playerData.money = 0;
        player.playerData.assetsWorth = 0;
        foreach (estateBoard board in mapList){
            if(board.owner==player){
                board.owner=bank;
            }

        }
        player.playerData.assetsList.Clear();
        Debug.Log($"{player.playerData.name} is bankrupt! All assets are repossessed!");
    }
    public void GoToJail(Player player)
    {
        if(mapList.FindIndex(board=>board.group == "Go to jail")>0){
        player.playerData.positionNo = mapList.FindIndex(board=>board.group == "Go to jail");
        }
        else {
            Debug.LogError("can't find board Go to jail");
        }
        FreezeTurn(player, 2);

        
        
      Debug.Log($"{player.name} is sent to jail!");
    }
    public void FreezeTurn(Player player, int turns){
        player.playerData.freezeTurn = turns;
    }
  
    
    public void BuyProperty(Player player,estateBoard board)
    {


        if (player.playerData.money >= board.price)
        {
            PayMoney(player,board.price);
            AddProperty(player,board);
            Debug.Log($"{player.playerData.name} bought {board.property}!");
        }
        else
        {
            Debug.Log($"{player.playerData.name} does not have enough money to buy {board.property}!");

        }}
        public void AddProperty(Player player, estateBoard board)
        {
            board.owner = player;
            player.playerData.assetsWorth+=board.price;
            player.playerData.assetsList.Add(board.property);

        }
       
   
        public void PayRent(Player player, estateBoard board)
        {
            if(!player.playerData.isBankrupt)
            {
                PayMoney(player, board.rent);       
                AddMoney(board.owner,board.rent);
                string _owner=board.owner.name;
                    
                
                Debug.Log($"{player.playerData.name} paid £{board.rent} in rent to {_owner}!");
            }
        }
        public void MoveTo(Player player, string boardName)
        {
            while (mapList[player.playerData.positionNo].property != boardName)
        {
            player.Move(1); 
        }
        }

     
    }

