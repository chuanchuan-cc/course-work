using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Text;

[System.Serializable]


public class PlayerDisplay: MonoBehaviour
{
    public Image playerImage;

    public Player player;


    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerWorth;
    public TextMeshProUGUI playerAssets;
  
   

    void Start(){



        ShowPlayer(player);

    }


    public void ShowPlayer(Player player)
    {
        if(player==null){
        Debug.LogError("no player!");
        return;
       }else{
       
      
        playerImage.sprite=player.GetComponent<SpriteRenderer>().sprite;
        playerName.text=player.name;
        playerWorth.text=player.playerData.assetsWorth.ToString();
        if(player.playerData.assetsList.Count==0){
             playerAssets.text="no asset";
            
             return;

            
           }else{
        
      
            
            string tempAssetslist = string.Join(", ", player.playerData.assetsList);
            playerAssets.text = tempAssetslist;
            
        
        }
        
       }

    }
    }
   
  
 

