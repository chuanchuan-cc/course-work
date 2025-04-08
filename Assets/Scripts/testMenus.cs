using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
public class testMenus : MonoBehaviour
{

    public Button somethingForNothing;
    public Button showMeTheMoney;
    public Button operationCWAL;
    public Button modifyThePhaseVariance;
    public TMP_InputField inputtext;
    public Button CheatRollButton;
    public GameObject menus;
    public CanvasGroup canvasGroup;
    public GameBehaviour gameBehaviour;
    public bool isCheating=false;
    private Player player;
    private List<Board> maplist;
    private List<Player>playerlist;



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
    CheatRollButton.onClick.AddListener(CheatRoll);
    isCheating=true;
}

public void cheating(Player _player){
    if(isCheating){
        close();
    }else{
        showMenus(_player);
        RunGame.instance.cheat();
    }

}
public void setmapplayer(List<Board> _maplist,List<Player> _playerlist){
    maplist=_maplist;
    playerlist=_playerlist;

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
    /*
    Board board=maplist[player.playerData.positionNo];
    estateBoard eBoard=board as estateBoard;
    int inMoney=player.playerData.money;
    if(eBoard!=null){
    if(player.playerData.assetsList.Contains(eBoard)){
        

        gameBehaviour.BuildBuilding(player,eBoard);
        

    }else{
        Debug.Log("it is not your property");
    }}else{
        Debug.Log("can not build in this property");
    }
    player.playerData.money=inMoney;
    
    foreach(Player p in playerlist){
    
            if(p.name==player.name){
            gameBehaviour.PayMoney(p,200);
            }
            else{
            gameBehaviour.PayMoney(p,100);
            }
        
    }

*/
player.playerData.money=50;


}
private void ModifyThePhaseVariance(){
    Debug.Log($"original circle is {player.playerData.circle}, now is {player.playerData.circle+1}");
    player.playerData.circle++;


}
private void CheatRoll(){
    int result;

    if (int.TryParse(inputtext.text, out result))
{
    RunGame.instance.cheatRoll(result);
}
else
{
    Debug.Log("fail to set step");
}
 
    
}

}
