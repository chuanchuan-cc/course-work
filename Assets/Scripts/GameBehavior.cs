using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class GameBehaviour: MonoBehaviour
{
   
    
 

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
        if(RunGame.mapList.FindIndex(board=>board.property == "Jail/Just visiting")>0){
      
        int i= RunGame.mapList.FindIndex(board=>board.property == "Jail/Just visiting");
        player.Move(i-player.playerData.positionNo);
        
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
    if (player.playerData.assetsList.Contains(board))
    {
        if (board.improvedLevel == 0) 
        {
            int sellPrice = board.price; 
            player.playerData.money += sellPrice;
            player.playerData.assetsList.Remove(board);
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
        if(player.playerData.circle>0)
        {
            Debug.Log($"{player.name} has not completed a full circuit of the board and cannot buy {board.property} yet!");
        return;
        }
        if(board.owner != null && board.owner != RunGame.bank)
        {
            Debug.Log($"{board.property} is already owned by {board.owner.GetName()} and cannot be purchased!");
        return;
        }

        if (player.playerData.money >= board.price)
        {
            PayMoney(player,board.price);
            board.owner = player.playerData;
            AddProperty(player,board);
            Debug.Log($"{player.name} bought {board.property}!");
        }
        else
        {
            Debug.Log($"{player.name} does not have enough money to buy {board.property}!");
            
            StartAuction(board);
            
        }}

        /*
        public int MakeBid(Player player, estateBoard board, int currentHighestBid)
        {
            
            if (RunGame.isAI)
            {
               // 我会写！不是现在搞！
               // return AIBidStrategy(board, currentHighestBid);
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
        */
     private void StartAuction(estateBoard board)
{
    Debug.Log($"Starting auction for {board.property}!");

    List<Player> bidders = new List<Player>();  

    foreach (Player player in RunGame.playersList)
    {
        if (player.playerData.circle > 0 && player.playerData.money > 0) 
        {
            bidders.Add(player);
        }
    }

    if (bidders.Count == 0)
    {
        Debug.Log($"No eligible players to bid for {board.property}. It remains unsold.");
        return;
    }

    int highestBid = 0;
    Player highestBidder = null;

    while (bidders.Count > 0)
    {
        List<Player> nextRoundBidders = new List<Player>();

        foreach (Player player in bidders)
        {
            int bid = GetPlayerBid(player, board, highestBid);

            if (bid > highestBid)
            {
                highestBid = bid;
                highestBidder = player;
            }

            if (bid > 0)
            {
                nextRoundBidders.Add(player);
            }
        }

        if (nextRoundBidders.Count <= 1)
        {
            break;
        }

        bidders = nextRoundBidders;
    }

    if (highestBidder != null)
    {
        PayMoney(highestBidder, highestBid);  
        board.owner = highestBidder.playerData;
        AddProperty(highestBidder, board);  
        Debug.Log($"{highestBidder.name} won the auction for {board.property} at £{highestBid}!");
    }
    else
    {
        Debug.Log($"No one placed a valid bid. {board.property} remains unsold.");
    }
}

        private int GetPlayerBid(Player player, estateBoard board, int currentHighestBid)
        {
             int bid = GetUserInputBid(player);
    
    if (bid > currentHighestBid && bid <= player.playerData.money)
    {
        return bid;
    }

    Debug.Log($"{player.name} entered an invalid bid or chose not to bid.");
    return 0;
        }

        private int GetUserInputBid(Player player)
{
    int bid = 0;
    string input = "200"; // 这里得用 UI 获取输入
    if (int.TryParse(input, out bid))
    {
        return bid;
    }
    return 0;
}



            
        
        public void AddProperty(Player player, estateBoard board)
        {
            Debug.Log($"{player.name} buy {board.property}");
            board.owner = player.playerData;
            player.playerData.assetsWorth+=board.price;
            player.playerData.assetsList.Add(board);

        }
        public void AddBuyable(Player player, BuyableBoard board)
        {
            Debug.Log($"{player.name} buy {board.property}");
            board.owner = player.playerData;
            player.playerData.assetsWorth+=board.price;
            player.playerData.assetsList.Add(board);

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
        
          public void PayBuyableRent(Player player, BuyableBoard board)
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
    if (player.playerData.assetsList.Contains(board))
    {
        if (PlayerOwnsFullSet(player, board)) // 玩家必须拥有同色套装
       { 
        //弹出选择面板，判断bool isbuild
        //此处默认为false
        bool isbuild=false;
            if (board.improvedLevel < 5&& isbuild) // 0-4: 建造房屋，5: 酒店
            {
                int buildCost = board.price;
                if (player.playerData.money >= buildCost )
                {
                    PayMoney(player, buildCost);
                    board.improvedLevel++;
                    string buildingType = board.improvedLevel == 5 ? "a Hotel" : "a House";
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
    //List<estateBoard> colorGroup = GetColorGroup(board);
   // return colorGroup.All(property => player.assetsList.Contains(board.property));
   bool b=true;
   foreach(estateBoard i in RunGame.mapList){
    if(i.group==board.group&&i.owner!=player){
        b=false;
        break;
    }   
   }
   return b;
   



}

     
    }

