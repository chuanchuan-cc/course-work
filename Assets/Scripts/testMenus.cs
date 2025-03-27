using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class testMenus : MonoBehaviour
{

    public Button somethingForNothing;
    public Button showMeTheMoney;
    public Button operationCWAL;
    public Button modifyThePhaseVariance;
    public GameObject menus;
    public CanvasGroup canvasGroup;
    public GameBehaviour gameBehaviour;
    public bool isCheating=false;
    private Player player;
    private List<Board> maplist;



 void Start(){
    if (canvasGroup == null){
            canvasGroup = menus.GetComponent<CanvasGroup>();
       
            if (canvasGroup == null)
            {
                canvasGroup = menus.AddComponent<CanvasGroup>(); 
            }
       
       }

            gameBehaviour = GameObject.Find("BehaviourPool").GetComponent<GameBehaviour>();
       
       close();
 }
private void close(){
    menus.SetActive(false);
    isCheating=false;
}
private void showMenus(Player _player){
    player=_player;

    menus.SetActive(true);
    somethingForNothing.onClick.AddListener(SomethingForNothing);
    showMeTheMoney.onClick.AddListener(ShowMeTheMoney);
    operationCWAL.onClick.AddListener(OperationCWAL);
    modifyThePhaseVariance.onClick.AddListener(ModifyThePhaseVariance);
    isCheating=true;
}

public void cheating(Player _player){
    if(isCheating){
        close();
    }else{
        showMenus(_player);
    }

}
public void setmap(List<Board> _maplist){
    maplist=_maplist;
}
private void SomethingForNothing(){
    foreach(Board board in maplist){
        estateBoard eBoard=board as estateBoard;
        if(eBoard!=null){
            gameBehaviour.AddProperty(player,eBoard);
        }else{
            BuyableBoard bBoard = board as BuyableBoard;
            if(bBoard!=null){
                gameBehaviour.AddBuyable(player,bBoard);
            }
        }
    }



}
private void ShowMeTheMoney(){
    gameBehaviour.AddMoney(player,10000);


}
private void OperationCWAL(){
    Board board=maplist[player.playerData.positionNo];
    estateBoard eBoard=board as estateBoard;
    int inMoney=player.playerData.money;
    if(eBoard!=null){
    if(player.playerData.assetsList.Contains(eBoard)){
        

        gameBehaviour.BuildBuilding(player,eBoard);
        

    }else{
        Debug.Log("未拥有此地产");
    }}else{
        Debug.Log("此地产不可建造");
    }
    player.playerData.money=inMoney;


}
private void ModifyThePhaseVariance(){
    Board board=maplist[player.playerData.positionNo];
    estateBoard eBoard=board as estateBoard;
    int inMoney=player.playerData.money;
    if(eBoard!=null){
    if(player.playerData.assetsList.Contains(eBoard)){
        int improvedLevel=eBoard.improvedLevel;
        for(int i=0;i<5-improvedLevel;i++){


        gameBehaviour.BuildBuilding(player,eBoard);}
    }else{
        Debug.Log("未拥有此地产");
    }}else{
        Debug.Log("此地产不可建造");
    }
    player.playerData.money=inMoney;


}

}
