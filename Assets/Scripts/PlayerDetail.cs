using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Text;

[System.Serializable]
public class PlayerImage
{
    public string playerName;
    public Sprite playerSprite;
}

public class PlayerDetail: MonoBehaviour
{
    public Image playerImage;
    public List<PlayerImage> playerImageList;
    private Dictionary<string, Sprite> playerImageMap;
    public Player player;


    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerAssetsValue;
    public TextMeshProUGUI playerAssets;
    public TextMeshProUGUI assetsNumber;
   

    void Start(){

        playerImageMap = new Dictionary<string, Sprite>();
        foreach (var entry in playerImageList)
        {
            playerImageMap[entry.playerName.ToLower()] = entry.playerSprite;
        }

        ShowPlayer(player);

    }


    void updateDashBoard(Player player){
        ShowPlayer(player);
    }
    void ShowPlayer(Player player)
    {
        if(player==null){
        Debug.LogError("no player!");
        return;
       }else{
       if (playerImageMap.TryGetValue(player.name.ToLower(), out Sprite selectedSprite))
        {
            playerImage.sprite = selectedSprite;
        }
        else
        {
            Debug.LogWarning(" {player.name} have no image");
        }
      
       
        playerName.text=player.name;
        playerAssetsValue.text=player.assetsValue.ToString();
        if(player.assetsList.Count==0){
             playerAssets.text="no asset";
            
             return;

            
           }else{
        
      
            
            string tempAssetslist = string.Join(", ", player.assetsList);
            playerAssets.text = tempAssetslist;
            
        
        }
        assetsNumber.text=player.assetsList.Count.ToString();
       }

    }
    }
   
  
 

