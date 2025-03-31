using System;
using System.Collections.Generic;
using System.IO;
using ExcelDataReader;
using UnityEngine;

[Serializable]
public class Card
{
    public string description; // Column A
    public string action;      // Column D


    // Fields parsed from action
    public string group=null;
    public bool isPay=false;
    public bool isMove=false;
    public bool isFoward=false;
    public bool isPayFine=false;
    public bool isJailFree=false;
    public bool isGoJail=false;
    public bool isPayByAll=false;
    public string payer=null;
    public string payee=null;
    public int houseRepair=0;
    public int hotelRepair=0;
    public bool isRepair=false;
    public int moneyAmount=0;
    public string destinationName=null;
    public bool isInteractable=false;
    public bool collectGo=false;
    public int steps;

    public Card() { }

    public Card(string description, string action)
    {
        this.description = FormatStr(description);
        this.action = FormatStr(action);
        CardRead();
    }

    private static string FormatStr(string str)
    {
        if (string.IsNullOrEmpty(str)) return "UNKNOWN";
        return str.Trim().ToLower();
    }

    private void CardRead()
    {
        if (description.ToLower().Contains("go to jail"))
        {
            isGoJail = true;
            if(description.ToLower().Contains("not collect"))
            collectGo=false;
            else
            collectGo=true;
        }
        else if (action.Contains("moves"))
        {
                        isMove = true;
            if(description.Contains("do not collect")){
                collectGo=false;
            }
            
            if(action.Contains("forwards")){
                isFoward=true;

                destinationName = action.Trim().Split(" to ")[1];
            }else if(action.Contains("backwards")){isFoward=false;
            destinationName = action.Trim().Split(" to ")[1];
            }else if (action=="player moves token"){
                if(description.Contains("to")){
      

                string[] parts = description.Trim().Split(". ");
                string[] parts2=parts[0].Split(" to ");
                destinationName=parts2[1].Replace("\"", "");
                
                }




                else if(description.Contains("go back")){
                    string[] parts=description.Split(' ');
                                    foreach (string n in parts)
                    {
                   if (int.TryParse(n, out int amount))
               {
                             steps = -1*amount;
                      break;

                 }
        }
                    

                }


            }

        }
        else if (action.Contains("pays"))
        {
            isPay = true;
            action=action.Replace("£","");
            string[] parts = action.Split(' ');
            int amount;
            if(description.Contains("repairs")){
                
                string[] _parts=description.Replace("You are assessed for repairs","" ).Replace("/house"," ").Replace("/hotel"," ").Replace("£"," ").Split(", ");
               if (_parts.Length >= 2)
            {
           foreach (string n in _parts)
        {
            string clean = n.Trim().Replace("\"", "");
            if (int.TryParse(clean, out amount))
            {
                if(houseRepair==0)
                houseRepair = amount;
                else{
                    hotelRepair=amount;
                    break;
                    
                }

            }
        }
                isRepair = true;
                    }
                isRepair=true;


            }
            

            else if (parts[0]=="bank" )
            {
                foreach (string n in parts)
        {
            if (int.TryParse(n, out amount))
            {
                moneyAmount = amount;
                break;

            }
        }
                payer="bank";
                payee="player";
                
                }
            
            else if (parts[0]=="player")
            {
                foreach (string n in parts)
        {
            if (int.TryParse(n, out amount))
            {
                moneyAmount = amount;
                break;

            }
        }
                payer = "player";
                payee = "bank";

                
        }}
        else if (action.Contains("each player"))
        {
            isPay = true;
            isPayByAll = true;
            int amount;
            action=action.Replace("£","");
            string[] parts = action.Split(' ');
           foreach (string n in parts)
        {
            if (int.TryParse(n, out amount))
            {
                moneyAmount = amount;
                break;

            }
        }}
            
        
        else if (action.Contains("free parking")){
     

            action=action.Replace("£","");
            string[] parts = action.Split(' ');
            int amount;
        
            isPayFine=true;

           
            
            
            
            if(description.Contains("or")){
                isInteractable=true;
               


            }
            foreach (string n in parts)
        {
            if (int.TryParse(n, out  amount))
            {
                moneyAmount = amount;
                break;

            }
        }

            
            
        
        }
        else if (description.ToLower().Contains("jail free")){
            isJailFree=true;
        }
    
    
    
    
    }}


 enum Group { None,PotLuck, OpportunityKnocks }

public static class CardLoader
{
    public static (List<Card>, List<Card>) LoadCards(string excelPath)
    {
        List<Card> luckCards = new List<Card>();
        List<Card> opportunityCards = new List<Card>();

        if (!File.Exists(excelPath))
        {
            Debug.LogError("Excel file does not exist: " + excelPath);
            return (luckCards, opportunityCards);
        }

        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            int rowIndex=0;
            bool isStarted = false;
            var currentGroup = Group.None;
            while(reader.Read())
            {
                rowIndex++;
               
   
        if (reader.FieldCount < 4 || reader.GetValue(0) == null)
        {
            continue;
        }

        string raw = reader.GetValue(0).ToString().Trim();

        if (raw == "Pot luck card data")
        {
            currentGroup = Group.PotLuck;
            isStarted = true;
            continue;
        }
        else if (raw == "Opportunity knocks card data")
        {
            currentGroup = Group.OpportunityKnocks;
            isStarted = true;
            continue;
        }


        if (!isStarted || raw == "Description")
        {
            continue;
        }

              try
            {
                string description = reader.GetValue(0)?.ToString() ?? "Unknown";
                string action = reader.GetValue(3)?.ToString() ?? "Unknown";

                Card card = new Card(description, action);

                if (currentGroup == Group.PotLuck)
                {
                    card.group = "Pot Luck";
                    luckCards.Add(card);
                }
                else if (currentGroup == Group.OpportunityKnocks)
                {
                    card.group = "Opportunity Knocks";
                    opportunityCards.Add(card);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error parsing card at row {rowIndex}: {ex.Message}");
            }
            }
        }

        Debug.Log($"Loaded {luckCards.Count} Pot Luck cards and {opportunityCards.Count} Opportunity Knocks cards.");
        return (luckCards, opportunityCards);
    }
}

