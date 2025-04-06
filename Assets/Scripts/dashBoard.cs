using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class dashBoardConstructor: MonoBehaviour
{

    public GameObject playerDetail; 
    public GameObject DashBoard; 
    

    void Start(){
        
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
    public void RebuildLayout()
{
    if (DashBoard==null) return;
    RectTransform rt=DashBoard.GetComponent<RectTransform>();
    if (rt!=null)
        StartCoroutine(RebuildCoroutine(rt));
}

private IEnumerator RebuildCoroutine(RectTransform rt)
{
    yield return null;
    LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
}

}


