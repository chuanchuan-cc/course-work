using UnityEngine;
using System.Collections.Generic;
using System.Collections;



public class GameBehaviour: MonoBehaviour
{
    public playerInteractionPanel interactionPanel;




   
   void Start(){
    interactionPanel=GameObject.Find("interactionPanel").GetComponent<playerInteractionPanel>();
    if(interactionPanel==null)Debug.Log("can't find interaction panel");
    
   }
    public void bankrupt(Player player){
        player.playerData.isBankrupt=true;
        foreach(Board b in player.playerData.assetsList){
            estateBoard eb = b as estateBoard;
            if(eb!=null)
                eb.owner=RunGame.bank;
            else{
                BuyableBoard bb = b as BuyableBoard;
                bb.owner=RunGame.bank;
            }
            

        }

    }

 

    public void AddMoney(Player player,int amount)
    {
        
        player.playerData.money += amount;
        Debug.Log($"{player.name} recieved £{amount}, new balance: £{player.playerData.money}");
        player.playerData.assetsWorth+=amount;
        MusicController.Instance.PlayMoneySound();
    }
    public void PayMoney(Player player,int amount)
    {
        if (player.playerData.isBankrupt)return;
        if (player.playerData.money >= amount)
        {
            player.playerData.money -= amount;
            Debug.Log($"{player.name} paid £{amount}, remaining balance: £{player.playerData.money}");
        player.playerData.assetsWorth-=amount;
        MusicController.Instance.PlayMoneySound();
        }
        
    }

    public void GoToJail(Player player)
    {
        
        
        
        Board b=RunGame.mapList.Find(board=>board.property == "Jail/Just visiting");

        if(b!=null){
            if(player.playerData.freeJail==0){
            MusicController.Instance.PlayJailSound();
             FreezeTurn(player, 2);}
      
        player.directlyMove(b);
        
        
        }
        else {
            Debug.LogError("can't find board Go to jail");
        }
        
       


        
        
      Debug.Log($"{player.name} is sent to jail!");
    }
    public void FreezeTurn(Player player, int turns){
        player.playerData.freezeTurn = turns;
    }
    public void SellEstateBoard(Player player, estateBoard board)
{
    if (player.playerData.assetsList.Contains(board))
    {
        if (board.improvedLevel == 0) 
        {
              int sellPrice;
            if(board.isMortgage){
                sellPrice=(board.price%2==0)? board.price/2:(board.price-1)/2;

            }
            else {sellPrice = board.price; }
            player.playerData.money += sellPrice;
            player.playerData.assetsList.Remove(board);
            board.owner=RunGame.bank;
            Debug.Log($"{player.name} sold {board.property} for £{sellPrice}.");
        }
        else
        {
            int sellPrice;
            if(board.isMortgage){
                sellPrice=((board.price-board.initialPrice)%2==0)? (board.price-board.initialPrice)/2:((board.price-board.initialPrice)-1)/2;

            }
            else {sellPrice = board.price-board.initialPrice; }
            player.playerData.money += sellPrice;
            board.price=board.initialPrice;
            board.rent=board.baseRent;
            board.improvedLevel=0;
            board.owner=RunGame.bank;

            Debug.Log($"{player.name} cannot sell {board.property} because it has buildings on it!");
        }
    }
    else
    {
        
        Debug.Log($"{player.name} does not own {board.property}, so they cannot sell it!");
    }

}
public void SellBuyableBoard(Player player,BuyableBoard board){
            int sellPrice;
            board.owner=RunGame.bank;
            if(board.isMortgage){
                sellPrice=(board.price%2==0)? board.price/2:(board.price-1)/2;

            }
            else {sellPrice = board.price; }
            player.playerData.money += sellPrice;
            player.playerData.assetsList.Remove(board);
            Debug.Log($"{player.name} sold {board.property} for £{sellPrice}.");


}
public void mortageEstateBoard(Player player,estateBoard board){
    board.isMortgage=true;
    player.playerData.money+=(board.price%2==0)? board.price/2:(board.price-1)/2;


}
public void mortageBuyableBoard(Player player,BuyableBoard board){
    board.isMortgage=true;
    player.playerData.money+=(board.price%2==0)? board.price/2:(board.price-1)/2;

    }
    
    public void remdeemEstateBoard(Player player,estateBoard board){
    board.isMortgage=false;
    player.playerData.money-=(board.price%2==0)? board.price/2:(board.price-1)/2;


}
public void remdeemBuyableBoard(Player player,BuyableBoard board){
    board.isMortgage=false;
    player.playerData.money-=(board.price%2==0)? board.price/2:(board.price-1)/2;


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

            if(board.group=="Utilities"){
              if(NumberOfOwnAssets(player,board)==0){
                 int rent=4*rollRent();
                 Debug.Log($"你摇出了rent {rent}");
                 board.setRent(rent);  

                     }    else{       
        int rent=10*rollRent();
                       Debug.Log($"你摇出了rent {rent}");
                                 board.setRent(rent);
           }  
            }else if(board.group=="Station"){
                switch (NumberOfOwnAssets(player,board)){
                    case 0:
                    board.setRent(25);
                    break;
                    case 1:
                    board.setRent(50);
                    break;
                    case 2:
                    board.setRent(100);
                    break;
                    case 3:
                    board.setRent(200);
                    break;
                   

                }
            }
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

public IEnumerator BuildBuilding(Player player, estateBoard board)
{




        Debug.Log("调用建筑特化操作面板");
        bool? userChoice=null;
        if(board.improvedLevel < 5){
        int buildCost = costCalculer(board);
        
        interactionPanel.ShowPanel($"are you want to pay {buildCost} to update you property? after that, this board price will be {board.price+buildCost}, rent will be {board.improvedRents[board.improvedLevel+1]}",board.group,board.price,board.rent,(bool isBuild)=> 
        { userChoice=isBuild;});
        yield return new WaitUntil(()=>userChoice.HasValue);
        
         if(userChoice.HasValue && userChoice.Value ){
            
                               
      if(player.playerData.money>buildCost){
         PayMoney(player, buildCost);
                    board.improvedLevel++;
                    board.ResetRent(board.improvedLevel);
                    board.price+=buildCost;
                    string buildingType = board.improvedLevel == 5 ? "a Hotel" : "a House";
                    Debug.Log($"{player.name} built {buildingType} on {board.property}.");
                    

                                
          }else{
             Debug.Log($"{player.name} does not have enough money to build on {board.property}!");
                                
                     
              }
             
             }
     else{
              //不升级
                      
          RunGame.instance.buildingButton.gameObject.SetActive(true);
                             
                       
         }}else{
            Debug.Log($"{board.property} is already fully developed with a Hotel!");
         

       }
  
       

    
}
    private int rollRent(){
        return Random.Range(1, 7);
    }
         /*
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
*/




/*IEnumerator buildingPanel(){
        bool? userChoice=null;
        interactionPanel.ShowPanel($"are you want to build? you will pay {} to update you property",eBoard.group,eBoard.price,eBoard.rent,(bool isBuilding)=> 
        { userChoice=isBuilding;});
        yield return new WaitUntil(()=>userChoice.HasValue);
         if(userChoice.HasValue && userChoice.Value){
                               
      if(player.playerData.money>eBoard.price){

                                 
          }else{
                                
                     
              }

             }
     else{

                      

                             
                       
         }
}
*/
private int costCalculer(estateBoard board){
    if(board.group.ToLower()=="brown"||board.group.ToLower()=="blue"){
        if(board.improvedLevel<5){
return 50;
        }else{
return 50+board.improvedRents[4];
        }
     
    }
    else    if(board.group.ToLower()=="purple"||board.group.ToLower()=="orange"){
        if(board.improvedLevel<5){
return 100;
        }else{
return 100+board.improvedRents[4];
        }
     
    }
    else    if(board.group.ToLower()=="red"||board.group.ToLower()=="yellow"){
        if(board.improvedLevel<5){
return 150;
        }else{
return 150+board.improvedRents[4];
        }

    
}else    if(board.group.ToLower()=="green"||board.group.Replace(" ","").ToLower()=="deepblue"){
        if(board.improvedLevel<5){
return 200;
        }else{
return 200+board.improvedRents[4];
        }

    }
    else {Debug.Log($"can't match you estateBoard, which group is {board.group.Replace(" ","").ToLower()}");
        return 0;
    }
}
public int cheatGetcost(estateBoard eBoard){
    return costCalculer(eBoard);

}
    private int NumberOfOwnAssets(Player player,Board b){
        
        int n =0;
        foreach(Board i in RunGame.mapList){
            BuyableBoard bB=i as BuyableBoard;
            if(bB!=null){
            if(bB.group==b.group&&bB.owner.GetName()==player.playerData.name){
                n++;
            }
        } 
        
        }
        return n;
       
    }








}

