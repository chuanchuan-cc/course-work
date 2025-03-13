using UnityEngine;

public class dashBoardConstructor: MonoBehaviour
{

    public GameObject playerDetail; 
    public GameObject DashBoard; 
    

    void start(){
        
    }



    public void CreateChildren(Player player)
    {
        DashBoard=GameObject.Find("DashBoard");
        if(DashBoard==null){
            Debug.Log("can't find DashBoard");
            return;}
        else Debug.Log("already find DashBoard");
            GameObject newPlayerDashBoard = GameObject.Instantiate(playerDetail, DashBoard.transform); 
            newPlayerDashBoard.name = player.name;
            PlayerDisplay playerDisplay=newPlayerDashBoard.GetComponent<PlayerDisplay>();
            playerDisplay.SetPlayer(player);

            Debug.Log($"already add {newPlayerDashBoard.name} in to the DashBoard");
          
        
    }
}


