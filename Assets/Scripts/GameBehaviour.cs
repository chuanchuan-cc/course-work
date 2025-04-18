using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;


public class GameBehaviour: MonoBehaviour
{
    public playerInteractionPanel interactionPanel;
    public bankPanel bankpanel;
    public CGcontrol cgControl;
    public TileGenerator generator;







   
   void Start(){
     //bind cg controller
    cgControl=  FindObjectOfType<CGcontrol>();
    generator=GameObject.Find("Map").GetComponent<TileGenerator>();



    interactionPanel=GameObject.Find("interactionPanel").GetComponent<playerInteractionPanel>();
    if(interactionPanel==null)Debug.Log("can't find interaction panel");
    
   }
    public void bankrupt(Player player){

        foreach(Board b in player.playerData.assetsList){
            estateBoard eb = b as estateBoard;
            if(eb!=null){
                eb.owner=RunGame.bank;
                generator.updateTile(eb);}
            else{
                BuyableBoard bb = b as BuyableBoard;
                bb.owner=RunGame.bank;
                generator.updateTile(bb);
            }
            

        }
        player.playerData.isBankrupt=true;
        player.gameObject.SetActive(false);
        RunGame.instance.deletePlayer(player);
        Destroy(player.gameObject);
        
        RunGame.instance.isNext = true;
        
        



    }

 

    public void AddMoney(Player player,int amount)
    {
        
        player.playerData.money += amount;
        Debug.Log($"{player.name} recieved £{amount}, new balance: £{player.playerData.money}");
        player.playerData.assetsWorth+=amount;
        MusicController.Instance.PlayMoneySound(); 
        cgControl.PlayCG("add_money",player);
        
        RunGame.instance. playerUpdate(player);
       
    }
    public void PayMoney(Player player,int amount)
    {
        Debug.Log($"{player.name} pay {amount}");
        if (player.playerData.isBankrupt)
        Debug.LogError("payer is bankrupted");
        if (player.playerData.money >= amount)
        {
            player.playerData.money -= amount;
            Debug.Log($"{player.name} paid £{amount}, remaining balance: £{player.playerData.money}");
        player.playerData.assetsWorth-=amount;
        MusicController.Instance.PlayMoneySound();
        cgControl.PlayCG("money_fly",player);
       
        }else{
            StartCoroutine(lackcash(player,amount));
            
                    }
        RunGame.instance. playerUpdate(player);
            

        
        
    }
private IEnumerator lackcash(Player player, int amount){
    if(player.playerData.isAI){ 
        if(player.playerData.assetsWorth>amount){                   
                            while(player.playerData.money<amount)
                            AISell(player,amount-player.playerData.money);
                            PayMoney(player, amount);}
                            else{
                            bankrupt(player);}
                    }
                    else{
                        
                            if(player.playerData.assetsWorth>amount){
                            bankpanel.showbankruptPanel(player,amount);
                            yield return new WaitUntil(()=>player.playerData.money>amount);
                            bankpanel.ClosePanel();
                            PayMoney(player, amount);
                            }
                            else{
                                bankrupt(player);
                            }

                        }
}
public void AIRedeem(Player player){

    foreach(Board board in player.playerData.assetsList){
        if(board is BuyableBoard bBoard){
            if(bBoard.isMortgage&&(player.playerData.money-bBoard.price/2)>1000)
            remdeemBuyableBoard(bBoard);
        }
        if(board is estateBoard eBoard){
            if(eBoard.isMortgage&&(player.playerData.money-eBoard.price/2)>1000)
            remdeemEstateBoard(eBoard);
        }
    }

}
    public void AISell(Player player,int i){
        int im=player.playerData.money;

        if (player.playerData.assetsList.Count == 0 || player.playerData.assetsWorth <= player.playerData.money) {
        bankrupt(player);
        return;}
    else if(RunGame.instance.difficulty==0){

         

        int n=Random.Range(0,player.playerData.assetsList.Count);
        estateBoard eBoard= player.playerData.assetsList[n] as estateBoard;
        if(Random.Range(0,2)==0){
    if(eBoard!=null){
        SellEstateBoard(eBoard);

    }
    else{
        BuyableBoard bBoard= player.playerData.assetsList[n] as BuyableBoard;
        SellBuyableBoard(bBoard);
    }}else{
         if(eBoard!=null){
            if(eBoard.improvedLevel==0){
            if(eBoard.isMortgage)
        SellEstateBoard(eBoard);
        else{
            mortageEstateBoard(eBoard);
        }}else{
            tearBuilding(eBoard);
        }

    }
    else{
        BuyableBoard bBoard= player.playerData.assetsList[n] as BuyableBoard;
        if(bBoard.isMortgage)
        SellBuyableBoard(bBoard);
        else{
            mortageBuyableBoard(bBoard);
        }
    }

    }


        

    }
    else if(RunGame.instance.difficulty==1){
 
    List<int>l2 = canMortagageList(player);
    if(l2.Count!=0){
      
            int n=Random.Range(0,l2.Count);
            estateBoard eBoard= player.playerData.assetsList[l2[n]] as estateBoard;
    if(eBoard!=null)
    mortageEstateBoard(eBoard);
    else{
        BuyableBoard bBoard= player.playerData.assetsList[l2[n]] as BuyableBoard;
        mortageBuyableBoard(bBoard);
    }

        


    }
    else{
    int n1=Random.Range(0,player.playerData.assetsList.Count);
    estateBoard eBoard= player.playerData.assetsList[n1] as estateBoard;
    if(eBoard!=null)
    SellEstateBoard(eBoard);
    else{
        BuyableBoard bBoard= player.playerData.assetsList[n1] as BuyableBoard;
        SellBuyableBoard(bBoard);
    }


    }
    


    }

    if(player.playerData.money<i)
    AISell(player,i-player.playerData.money);
   

    RunGame.instance.playerUpdate(player);





}
private List<int> canMortagageList(Player player){
    List<int> l1=new List<int>();
    int t=0;
foreach(Board board in player.playerData.assetsList){
estateBoard eBoard= board as estateBoard;
if(eBoard!=null){
    if(!eBoard.isMortgage&&eBoard.improvedLevel==0)
    l1.Add(t);
}else{
BuyableBoard bBoard= board as BuyableBoard;
if(!bBoard.isMortgage)
l1.Add(t);

}
t++;
}

return l1;
}

    public IEnumerator GoToJail(Player player)
    {
        
        
        
        Board b=RunGame.mapList.Find(board=>board.property == "Jail/Just visiting");

        if(b!=null){
            if(player.playerData.isAI){
                if(player.playerData.freeJail>0)
                player.playerData.freeJail--;
                else if(player.playerData.money>1000){
                    PayMoney(player,50);

                }else{cgControl.PlayCGAnimation("jail",new Vector3(999999f,999999f,999999f));
            MusicController.Instance.PlayJailSound();
             FreezeTurn(player, 2);
            

                }

            }else{
                
            if(player.playerData.freeJail==0){
                bool? userChoice=null;


        interactionPanel.ShowPanel($"are you want to pay 50 to released from jail?",(bool Result)=> 
          { userChoice=Result;});

          yield return new WaitUntil(()=>userChoice.HasValue);
               if(userChoice.Value){
                PayMoney(player,50);
                 }else{
            cgControl.PlayCGAnimation("jail",new Vector3(999999f,999999f,999999f));
            MusicController.Instance.PlayJailSound();
             FreezeTurn(player, 2);}

              }
             
       
               
             else
             player.playerData.freeJail--;
      
        
        
        
        }
        player.directlyMove(b);
        yield return new WaitUntil(()=>!cgControl.isCG);
        
        }
        else {
            Debug.LogError("can't find board Go to jail");
        }
        
       


        
        
      Debug.Log($"{player.name} is sent to jail!");
    }

    public void FreezeTurn(Player player, int turns){
        player.playerData.freezeTurn = turns;
    }
    public void SellEstateBoard(estateBoard board)
{
   
    if (board.owner is PlayerData playerOwner){
    if (playerOwner.assetsList.Contains(board)){

        if (board.improvedLevel == 0) 
        {
              int sellPrice;
            if(board.isMortgage){
                sellPrice=(board.price%2==0)? board.price/2:(board.price-1)/2;

            }
            else {sellPrice = board.price; }
            playerOwner.money += sellPrice;
            playerOwner.assetsList.Remove(board);
            board.owner=RunGame.bank;
           

            foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }
            
            Debug.Log($"{playerOwner.name} sold {board.property} for £{sellPrice}.");
        }
        else
        {

            tearBuilding(board);
          
        }
    }
    else
    {
        
        Debug.Log($"{playerOwner.name} does not own {board.property}, so they cannot sell it!");
    }}
    generator.updateTile(board);
}


public void SellBuyableBoard(BuyableBoard board){
            int sellPrice;
            if( board.owner is PlayerData playerOwner){
        
            if(board.isMortgage){
                sellPrice=(board.price%2==0)? board.price/2:(board.price-1)/2;

            }
            else {sellPrice = board.price; }
            playerOwner.money += sellPrice;
            playerOwner.assetsList.Remove(board);

            Debug.Log($"{playerOwner.name} sold {board.property} for £{sellPrice}.");
            foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }
            }
board.owner=RunGame.bank;
generator.updateTile(board);


}
public void mortageEstateBoard(estateBoard board){

  

    board.isMortgage=true;
    if( board.owner is PlayerData playerOwner){
    playerOwner.money+=(board.price%2==0)? board.price/2:(board.price-1)/2;

    foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }
    }


}
public void mortageBuyableBoard(BuyableBoard board){
 

    board.isMortgage=true;
    if( board.owner is PlayerData playerOwner){
    playerOwner.money+=(board.price%2==0)? board.price/2:(board.price-1)/2;
    foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }
    }
    }
    
    public void remdeemEstateBoard(estateBoard board){

    board.isMortgage=false;
    if( board.owner is PlayerData playerOwner){
    playerOwner.money-=(board.price%2==0)? board.price/2:(board.price-1)/2;
    foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }

    }
}
public void remdeemBuyableBoard(BuyableBoard board){

    board.isMortgage=false;
    if( board.owner is PlayerData playerOwner){
    playerOwner.money-=(board.price%2==0)? board.price/2:(board.price-1)/2;
    foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }}


}


    

        


            
        
        public void AddProperty(Player player, estateBoard board)
        {
            if(!player.playerData.assetsList.Contains(board)){
            Debug.Log($"{player.name} buy {board.property}");
            board.owner = player.playerData;
            player.playerData.assetsWorth+=board.price;
            player.playerData.assetsList.Add(board);
            RunGame.instance.playerUpdate(player);
            }
            generator.updateTile(board);

        }
        public void AddBuyable(Player player, BuyableBoard board)
        {
            if(!player.playerData.assetsList.Contains(board)){
            Debug.Log($"{player.name} buy {board.property}");

            if(board.group=="Utilities"){
              if(NumberOfOwnAssets(player,board)==0){
                 int rent=4*rollRent();
                 Debug.Log($"roll rent {rent}");
                 board.setRent(rent);  

                     }    else{       
        int rent=10*rollRent();
                       Debug.Log($"roll rent {rent}");
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
            generator.updateTile(board);


        }}
       
   
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
        public void tearBuilding(estateBoard board){
            board.improvedLevel-=1;
            int n;
            if(board.owner is PlayerData playerOwner){
            if(board.isMortgage)
            n=(costCalculer(board))/2;
            else
            n=costCalculer(board);
            playerOwner.money+=n;
            RunGame.bank.money-=n;
            foreach(Player player in RunGame.instance.getplayerlist()){
                if(player.name==playerOwner.name)
                RunGame.instance. playerUpdate(player);
            }
            
            }
            generator.updateTile(board);
        }

public IEnumerator BuildBuilding(Player player, estateBoard board)
{




      
        bool? userChoice=null;
        if(board.improvedLevel < 5){
        int buildCost = costCalculer(board);
        
        interactionPanel.ShowPanel($"are you want to pay {buildCost} to update you property? after that, this board price will be {board.price+buildCost}, rent will be {board.improvedRents[board.improvedLevel]}",board.group,board.price,board.rent,(bool isBuild)=> 
        { userChoice=isBuild;});
        yield return new WaitUntil(()=>userChoice.HasValue);
        
         if(userChoice.HasValue && userChoice.Value ){
            
                               
      if(player.playerData.money>buildCost){
         
                    board.improvedLevel++;
                    board.ResetRent(board.improvedLevel-1);
                    board.price+=buildCost;
                    string buildingType = board.improvedLevel == 5 ? "a Hotel" : "a House";
                    Debug.Log($"{player.name} built {buildingType} on {board.property}.");
                    PayMoney(player, buildCost);
                    cgControl.PlayCG("add_money",player);
                    

                                
          }else{
             Debug.Log($"{player.name} does not have enough money to build on {board.property}!");
                                
                     
              }
             
             }
     else{
              //do not update
                      
          RunGame.instance.buildingButton.gameObject.SetActive(true);
                             
                       
         }}else{
            Debug.Log($"{board.property} is already fully developed with a Hotel!");
         

       }
       generator.updateTile(board);
  
       

    
}
    private int rollRent(){
        return Random.Range(1, 7);
    }
         /*
            if (board.improvedLevel < 5&& isbuild) 
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
    public int costCalculer(estateBoard board){
    if(board.group.ToLower()=="brown"||board.group.ToLower()=="blue"){
        if(board.improvedLevel<4){
        return 50;
        }else{
        return 50+board.improvedRents[3];
        }
     
    }
    else if(board.group.ToLower()=="purple"||board.group.ToLower()=="orange"){
        if(board.improvedLevel<4){
            return 100;
        }else{
            return 100+board.improvedRents[3];
        }
     
    }
    else if(board.group.ToLower()=="red"||board.group.ToLower()=="yellow"){
        if(board.improvedLevel<4){
            return 150;
            }else{
                return 150+board.improvedRents[3];
            }

    
        }else if(board.group.ToLower()=="green"||board.group.Replace(" ","").ToLower()=="deepblue"){
            if(board.improvedLevel<4){
                return 200;
                }else{
                    return 200+board.improvedRents[3];
                }

        }
        else {Debug.Log($"can't match you estateBoard, which group is {board.group.Replace(" ","").ToLower()}");
            return 0;
        }
    }


public int getprice(estateBoard board, int improvedLevel){
    if(board.group.ToLower()=="brown"||board.group.ToLower()=="blue"){
        if(improvedLevel<4){
return 50;
        }else{
return 50+board.improvedRents[3];
        }
     
    }
    else    if(board.group.ToLower()=="purple"||board.group.ToLower()=="orange"){
        if(improvedLevel<4){
return 100;
        }else{
return 100+board.improvedRents[3];
        }
     
    }
    else    if(board.group.ToLower()=="red"||board.group.ToLower()=="yellow"){
        if(improvedLevel<4){
return 150;
        }else{
return 150+board.improvedRents[3];
        }

    
}else    if(board.group.ToLower()=="green"||board.group.Replace(" ","").ToLower()=="deepblue"){
        if(improvedLevel<4){
return 200;
        }else{
return 200+board.improvedRents[3];
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

