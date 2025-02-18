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

public class PlayerDisplay: MonoBehaviour
{
    public Image playerImage;
    public List<PlayerImage> playerImageList;
    private Dictionary<string, Sprite> playerImageMap;
    public Player player;


    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerWorth;
    public TextMeshProUGUI playerAssets;
  
   

    void Start(){

        playerImageMap = new Dictionary<string, Sprite>();
        foreach (var entry in playerImageList)
        {
            playerImageMap[entry.playerName.ToLower()] = entry.playerSprite;
        }

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
            Debug.LogWarning($" {player.name} have no image");
        }
      
       
        playerName.text=player.name;
        playerWorth.text=player.assetsWorth.ToString();
        if(player.assetsList.Count==0){
             playerAssets.text="no asset";
            
             return;

            
           }else{
        
      
            
            string tempAssetslist = string.Join(", ", player.assetsList);
            playerAssets.text = tempAssetslist;
            
        
        }
        
       }

    }
    }
   
  
 

