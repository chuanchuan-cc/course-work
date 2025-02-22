using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

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

    public static (List<Card>,List<Card>) LoadCards(string csvPath)
    {
        List<Card> luckCards = new List<Card>();
        List<Card> opportunityCards = new List<Card>();

        if (!File.Exists(csvPath))
        {
            Debug.LogError("path do not exist: " + csvPath);
            return (luckCards,opportunityCards);
        }

        string[] lines = File.ReadAllLines(csvPath);


        for (int i = 5; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line))
                continue;

            string[] cols = line.Split(',');

        
            string description = cols[0].Trim();
            string group = cols[5].Trim();
            bool isPay = (cols[6].Trim().ToUpper() == "TRUE");
            bool isPayForRepair = (cols[7].Trim().ToUpper() == "TRUE");
            bool isPayByAll = (cols[8].Trim().ToUpper() == "TRUE");
            bool isMove = (cols[9].Trim().ToUpper() == "TRUE");
            bool isFoward = (cols[10].Trim().ToUpper() == "TRUE");
            bool isPassGo = (cols[11].Trim().ToUpper() == "TRUE");
            bool isOperable = (cols[12].Trim().ToUpper() == "TRUE");
            bool isPayFine = (cols[13].Trim().ToUpper() == "TRUE");
            bool isJailFree = (cols[14].Trim().ToUpper() == "TRUE");
            bool isGoJail = (cols[15].Trim().ToUpper() == "TRUE");
            string payer = cols[16].Trim();
            string payee = cols[17].Trim();
            int money = 0;
            int.TryParse(cols[18].Trim(), out money);
            string destinationName = cols[19].Trim();
            

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

        return (luckCards,opportunityCards);
    }
}
