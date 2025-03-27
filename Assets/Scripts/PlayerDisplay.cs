using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using System.Text;
using System.Linq; 

[System.Serializable]


public class PlayerDisplay: MonoBehaviour
{
    public Image playerImage;

    public Player player;
    public TextMeshProUGUI playerCash;


    public TextMeshProUGUI playerName;
    public TextMeshProUGUI playerWorth;
    public TextMeshProUGUI playerAssets;
    public Transform PlayersPool;
    




  
   

   public void SetPlayer(Player newPlayer)
    {
        if (newPlayer == null)
        {
            Debug.LogError("Player is null!");
            return;
        }
        PlayersPool=GameObject.Find("PlayersPool").transform;

        player = newPlayer;
        playerName.text = player.name;
        playerImage.sprite=PlayersPool.Find(player.name).GetComponent<SpriteRenderer>().sprite;
     
        UpdateDisplay(player);


   
        
       }

    
    public void UpdateDisplay(Player player)
    {
        if (player == null)
        {
            Debug.LogError("No player to update!");
            return;
        }
        playerCash.text= player.playerData.money.ToString();
        
        playerWorth.text = player.playerData.assetsWorth.ToString();

        if (player.playerData.assetsList.Count == 0)
        {
            playerAssets.text = "No asset";
        }
        else
        {
            playerAssets.text = string.Join(", ", player.playerData.assetsList.Select(board => board.property));
        }

      
    }

    }
   
  
 

