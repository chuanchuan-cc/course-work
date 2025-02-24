using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using ExcelDataReader;
using UnityEngine;

[Serializable]
public class Card
{

     public string description;
     public string group;
     public bool isPay;
     public bool isPayForRepair;
     public bool isPayByAll;
     public bool isMove;
     public bool isFoward;
     public bool isPassGo;
     public bool isOperable;
     public bool isPayFine;
     public bool isJailFree;
     public bool isGoJail;
     public int moneyAmount;  
     public string destinationName;
     public string payer;
     public string payee;

  
    public Card() { }

    public Card(
        string description,
        string group,
        bool isPay,
        bool isPayForRepair,
        bool isPayByAll,
        bool isMove,
        bool isFoward,
        bool isPassGo,
        bool isOperable,
        bool isPayFine,
        bool isJailFree,
        bool isGoJail,
        int moneyAmount,       
        string destinationName,
        string payer,
        string payee
    )
    {
        this.group = group;
        this.description = description;
        this.isPay = isPay;
        this.isPayForRepair=isPayForRepair;
        this.isPayByAll=isPayByAll;
        this.isMove = isMove;
        this.isFoward=isFoward;
        this.isPassGo=isPassGo;
        this.isOperable=isOperable;
        this.isPayFine=isPayFine;
        this.isGoJail = isGoJail;
        this.isJailFree = isJailFree;
        this.moneyAmount = moneyAmount;
        this.destinationName = destinationName;
        this.payer = payer;
        this.payee = payee;
    }
    

}


public static class CardLoader
{

    public static (List<Card>,List<Card>) LoadCards(string excelPath)
    {
        List<Card> luckCards = new List<Card>();
        List<Card> opportunityCards = new List<Card>();

        if (!File.Exists(excelPath))
        {
            Debug.LogError("excel file do not exist: " + excelPath);
            return (luckCards,opportunityCards);
        }


        using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream)){
        {
            int rowIndex=0;
            while(reader.Read()){
            if (reader.GetValue(5)==null){
                rowIndex++;
                continue;
            }
      

        
                string description = reader.GetValue(0)?.ToString() ?? "Unknown";
                string group = reader.GetValue(5)?.ToString() ?? "Unknown";

                bool isPay = bool.TryParse(reader.GetValue(6)?.ToString(), out bool pay) ? pay : false;
                bool isPayForRepair = bool.TryParse(reader.GetValue(7)?.ToString(), out bool payForRepair) ? payForRepair : false;
                bool isPayByAll = bool.TryParse(reader.GetValue(8)?.ToString(), out bool payByAll) ? payByAll : false;
                bool isMove = bool.TryParse(reader.GetValue(9)?.ToString(), out bool move) ? move : false;
                bool isFoward = bool.TryParse(reader.GetValue(10)?.ToString(), out bool foward) ? foward : false;
                bool isPassGo = bool.TryParse(reader.GetValue(11)?.ToString(), out bool passGo) ? passGo : false;
                bool isOperable = bool.TryParse(reader.GetValue(12)?.ToString(), out bool operable) ? operable : false;
                bool isPayFine = bool.TryParse(reader.GetValue(13)?.ToString(), out bool payFine) ? payFine : false;
                bool isJailFree = bool.TryParse(reader.GetValue(14)?.ToString(), out bool jailFree) ? jailFree : false;
                bool isGoJail = bool.TryParse(reader.GetValue(15)?.ToString(), out bool goJail) ? goJail : false;

                string payer = reader.GetValue(16)?.ToString() ?? "Unknown";
                string payee = reader.GetValue(17)?.ToString() ?? "Unknown";
                int money = int.TryParse(reader.GetValue(18)?.ToString(), out int moneyValue) ? moneyValue : 0;
                string destinationName = reader.GetValue(19)?.ToString() ?? "Unknown";
            

            Card c= new Card(
                description,
                group,
                isPay,
                isPayForRepair,
                isPayByAll,
                isMove,
                isFoward,
                isPassGo,
                isOperable,
                isPayFine,
                isJailFree,
                isGoJail,
                money,
                destinationName,
                payer,
                payee
            );

            if(c.group.ToUpper()=="POT LUCK")luckCards.Add(c);
            else if(c.group.ToUpper()=="OPPORTUNITY KNOCKS")opportunityCards.Add(c);
        }
        //此处写检查卡牌有效性办法
        }
        return (luckCards,opportunityCards);
    }
}
}
