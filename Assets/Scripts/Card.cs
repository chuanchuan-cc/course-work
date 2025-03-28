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
    public bool collectGo=true;

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
        }
        else if (action.Contains("moves"))
        {
                        isMove = true;
            if(description.Contains("do not collect")){
                collectGo=false;
            }
            
            if(action.Contains("forwards")){
                isFoward=true;
                destinationName = action.Replace("player moves forwards to ", "").Trim();
            }else if(action.Contains("backwards")){isFoward=false;
            destinationName = action.Replace("player token moves backwards to ", "").Trim();
            }else if (action=="player moves token"){
                description.Replace("advance to ", "").Trim();
                string[] parts = description.Split('.');
                destinationName=parts[0];


            }

        }
        else if (action.Contains("pays"))
        {
            isPay = true;
            action=action.Replace("£","");
            string[] parts = action.Split(' ');
            int amount;
            if(action=="player pays money to the bank"){
                description.Replace("You are assessed for repairs","" ).Replace("/house","").Replace("/hotel","").Replace("£","");
                string[] _parts=description.Split(", ");
               if (_parts.Length >= 2)
            {
            int.TryParse(_parts[0], out houseRepair);
            int.TryParse(_parts[1], out hotelRepair);
                isRepair = true;
                    }
                isRepair=true;


            }
            

            else if (parts[0]=="bank" )
            {
                if(parts.Length>=4){
                int.TryParse(parts[3], out amount);
                payer="bank";
                payee="player";
                moneyAmount = amount;
                }
            }
            else if (parts[0]=="player")
            {
                if(parts.Length>=3){
                    int.TryParse(parts[2], out amount);
                payer = "player";
                payee = "bank";
                moneyAmount=amount;
                }
        }}
        else if (action.Contains("each player"))
        {
            isPay = true;
            isPayByAll = true;
            int amount;
            action=action.Replace("£","");
            string[] parts = action.Split(' ');
            if (parts.Length >= 3){

            if (int.TryParse(parts[2], out amount))
            {
                moneyAmount = amount;
            }}}
            
        
        else if (action.Contains("free parking")){
     

            action=action.Replace("£","");
            string[] parts = action.Split(' ');
            int amount;
        
            isPayFine=true;

            if(parts.Length >= 3 && int.TryParse(parts[2], out amount)){
            
            moneyAmount=amount;
            }
            if(description.Contains("or")){
                isInteractable=true;
            }
            
        
        }
        else if (description=="get out of jail free"){
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
            bool isStarted = false;
            var currentGroup = Group.None;
            while(reader.Read())
            {
               
   
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

                string description = reader.GetValue(0)?.ToString() ?? "Unknown";
                string action = reader.GetValue(3)?.ToString() ?? "Unknown";
                

                Card card = new Card(description, action);

                if (currentGroup==Group.PotLuck) {
                    card.group="Pot Luck";
                    luckCards.Add(card);}
                else if (currentGroup==Group.OpportunityKnocks) {
                    card.group="Opportunity Knocks";
                    opportunityCards.Add(card);
                
                }
            }
        }

        Debug.Log($"Loaded {luckCards.Count} Pot Luck cards and {opportunityCards.Count} Opportunity Knocks cards.");
        return (luckCards, opportunityCards);
    }
}

