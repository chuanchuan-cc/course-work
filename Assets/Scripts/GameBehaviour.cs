using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class GameBehaviour: MonoBehaviour
{
    public Player bank;
    
 

    public void AddMoney(Player player,int amount)
    {
        
        player.playerData.money += amount;
        Debug.Log($"{player.name} recieved £{amount}, new balance: £{player.playerData.money}");
        player.playerData.assetsWorth+=amount;
    }
    public void PayMoney(Player player,int amount)
    {
        if (player.playerData.isBankrupt)return;
        if (player.playerData.money >= amount)
        {
            player.playerData.money -= amount;
            Debug.Log($"{player.name} paid £{amount}, remaining balance: £{player.playerData.money}");
        player.playerData.assetsWorth-=amount;
        }
        
    }
    private void HandleBankruptcy(Player player)
    {
        player.playerData.isBankrupt = true;
        player.playerData.money = 0;
        player.playerData.assetsWorth = 0;
        foreach (estateBoard board in RunGame.mapList){
            if(board.owner==player){
                board.owner=RunGame.bank;
            }

        }
        player.playerData.assetsList.Clear();
        Debug.Log($"{player.name} is bankrupt! All assets are repossessed!");
    }
    public void GoToJail(Player player)
    {
        if(RunGame.mapList.FindIndex(board=>board.group == "Go to jail")>0){
      
        player.playerData.positionNo = RunGame.mapList.FindIndex(board=>board.group == "Go to jail");
        
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
    public void SellProperty(Player player, estateBoard board)
{
    if (player.ownedProperties.Contains(board))
    {
        if (board.buildingLevel == 0) 
        {
            int sellPrice = board.price; 
            player.playerData.money += sellPrice;
            player.ownedProperties.Remove(board);
            Debug.Log($"{player.name} sold {board.property} for £{sellPrice}.");
        }
        else
        {
            Debug.Log($"{player.name} cannot sell {board.property} because it has buildings on it!");
        }
    }
    else
    {
        Debug.Log($"{player.name} does not own {board.property}, so they cannot sell it!");
    }
}
  
    
    public void BuyProperty(Player player,estateBoard board)
    {
        if(!plyer.isPassGo)
        {
            Debug.Log($"{player.name} has not completed a full circuit of the board and cannot buy {board.property} yet!");
        return;
        }
        if(board.owner != null && board.owner != RunGame.bank)
        {
            Debug.Log($"{board.property} is already owned by {board.owner.name} and cannot be purchased!");
        return;
        }

        if (player.playerData.money >= board.price)
        {
            PayMoney(player,board.price);
            board.owner = player;
            AddProperty(player,board);
            Debug.Log($"{player.name} bought {board.property}!");
        }
        else
        {
            Debug.Log($"{player.name} does not have enough money to buy {board.property}!");
            StartAuction(board, allPlayers, bank);
        }}

        public int MakeBid(Player player, estateBoard board, int currentHighestBid)
        {
            if (this.isAI)
            {
                return AIBidStrategy(board, currentHighestBid);
            }
            else
            {
                Debug.Log($"{name}, please enter your bid for {board.property}. The current highest bid is £{currentHighestBid}.");
        string input = GetPlayerInput(); //这里需要定义 UI?,或者固定加钱
        int bid;
          if (int.TryParse(input, out bid) && bid > currentHighestBid && bid <= playerData.money)
            
            return bid;
        
        else
        {
            Debug.Log($"{name} entered an invalid bid or chose not to bid.");
            return 0; 
        }
            }
        }
        private void StartAuction(estateBoard board, List<Player>allPlayers)
        {
            Debug.Log(($"Starting auction for {board.property}!"));
            
            Player highestBidder = null;
            int highestBid = 0;
            List<Player>eligiblePlayers = allPlayers.Where(p => p.isPassGo && playerData.money > 0).ToList();
            if(eligiblePlayers.Count == 0)
            {
                Debug.Log($"No eligible players to bid for {board.property}. It remains unsold.");
        return;
    }
            
            foreach (var player in eligiblePlayers )
            {
                int bid = player.MakeBid(board, highestBid);
                if (bid > highestBid && bid <= player.playerData.money)
                {
                    highestBid = bid;
                    highestBidder = player;
                }
                
                if (highestBidder != null)
                {
                    PayMoney(highestBidder, highestBid);
                    board.owner = highestBidder;
                    highestBidder.ownedProperties.Add(board);
                    Debug.Log($"{highestBidder.name} won the auction for {board.property} at £{highestBid}!");
    }
    else
    {
        Debug.Log($"No one bid for {board.property}. It remains unsold.");
    }
                }
            }
        
        public void AddProperty(Player player, estateBoard board)
        {
            board.owner = player.playerData;
            player.playerData.assetsWorth+=board.price;
            player.playerData.assetsList.Add(board.property);

        }
       
   
        public void PayRent(Player player, estateBoard board)
        {
            if(!player.playerData.isBankrupt)
            {
                PayMoney(player, board.rent);
                foreach(Player cp in RunGame.playersList){
                    if(cp.name==board.owner.GetName()){
                        AddMoney(cp,board.rent);
                    }
                }
                string _owner=board.owner.GetName();
                    
                
                Debug.Log($"{player.name} paid £{board.rent} in rent to {_owner}!");
            }
        }
        public void MoveTo(Player player, string boardName)
        {
            while (RunGame.mapList[player.playerData.positionNo].property != boardName)
        {
            player.Move(1); 
        }
        }
        public void BuildBuilding(Player player, estateBoard board)
{
    if (player.ownedProperties.Contains(board))
    {
        if (PlayerOwnsFullSet(player, board)) // 玩家必须拥有同色套装
       { 
        //弹出选择面板，判断bool isbuild

        bool isbuild;
            if (board.buildingLevel < 5&& isbuild) // 0-4: 建造房屋，5: 酒店
            {
                int buildCost = board.houseCost;
                if (player.playerData.money >= buildCost )
                {
                    PayMoney(player, buildCost);
                    board.buildingLevel++;
                    string buildingType = board.buildingLevel == 5 ? "a Hotel" : "a House";
                    Debug.Log($"{player.name} built {buildingType} on {board.property}.");
                }
                else
                {
                    Debug.Log($"{player.name} does not have enough money to build on {board.property}!");
                }
            }
            else
            {
                Debug.Log($"{board.property} is already fully developed with a Hotel!");
            }
        }
        else
        {
            Debug.Log($"{player.name} cannot build on {board.property} because they do not own all properties in this color set!");
        }
    }
    else
    {
        Debug.Log($"{player.name} does not own {board.property}, so they cannot build on it!");
    }
}


private bool PlayerOwnsFullSet(Player player, estateBoard board)
{
    List<EstateBoard> colorGroup = GetColorGroup(board);
    return colorGroup.All(property => player.ownedProperties.Contains(property));
}

     
    }

