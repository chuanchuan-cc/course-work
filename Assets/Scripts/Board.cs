using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.IO;
using ExcelDataReader;
using Newtonsoft.Json;


[Serializable]
public class Board{

    public int positionNo;
    public string property;
    public string group;
    public string action;
    public bool canBeBought;
    
    public Board(int positionNo,string property, string group,string action, bool canBeBought){
        this.positionNo=positionNo;
        this.canBeBought=canBeBought;
        this.group=group;
        this.action=action;
        this.property=property;
    }
        public virtual Board DeepCopy()
    {
        return new Board(positionNo, property, group, action, canBeBought);
    }
   

}
[Serializable]
public class estateBoard:Board{
    public int improvedLevel;
    public int price;
    public int baseRent;
    public int[] improvedRents;
    public int rent;
    public bool isMortgage;
    [JsonIgnore]
    public IOwner owner;
    public int initialPrice;
    public estateBoard(int positionNo, string property, string group, 
                      int price, int baseRent, int[] improvedRents) 
        : base(positionNo, property, group, "", true)
    {
        ValidateConstruction(group, price, baseRent, improvedRents);

        this.improvedLevel=0;
        this.price=price;
        this.baseRent=baseRent;
        this.improvedRents=improvedRents;
        this.owner=RunGame.bank;
        this.rent=baseRent;
        this.isMortgage=false;
        this.initialPrice=price;
    

    }
      private void ValidateConstruction(string group, int price, int baseRent, int[] improvedRents)
    {
        if (!canBeBought)
            throw new ArgumentException("Estate boards must be buyable");
        
        if (improvedRents.Length != 5)
            throw new ArgumentException("Invalid improved rents array length");
    }
      public void ResetRent( int i){
        this.rent=this.improvedRents[i];
        
    }
     public override Board DeepCopy()
    {
        return new estateBoard(positionNo, property, group, price, baseRent, (int[])improvedRents.Clone())
        {
            improvedLevel=this.improvedLevel,
            rent=this.rent,
            isMortgage=this.isMortgage,
            initialPrice=this.initialPrice
        };
}}

[Serializable]
public class BuyableBoard : Board
{
    public int price;
    [JsonIgnore]
    public IOwner owner;
    public int rent;
    public bool isMortgage;

    public BuyableBoard(int positionNo, string property, string group, int price)
        : base(positionNo, property, group, "", true)
    {
        this.price=price;
        this.owner=RunGame.bank;
        if(group=="Station")this.rent=25;
        isMortgage=false;
    }
    public void setRent(int _rent){
        this.rent=_rent;
    }
    public override Board DeepCopy()
    {
        return new BuyableBoard(positionNo, property, group, price)
        {
            rent=this.rent,
            isMortgage=this.isMortgage
        };
    }
}


public static class BoardLoader
{
    public static List<Board> LoadBoards(string excelPath)
    {
        List<Board> boards=new List<Board>();

        try
        {
            
            using (var stream=File.Open(excelPath, FileMode.Open, FileAccess.Read))
            using (var reader=ExcelReaderFactory.CreateReader(stream))
            {
                int rowIdx=0;
                while(reader.Read())
                {
                    rowIdx++;
                if(!int.TryParse(reader.GetValue(0)?.ToString(), out _)){
                    continue;
                }
                else
                {
                    
                    
                    int positionNo=ParseInt(reader.GetValue(0))-1;
                    string property=GetString(reader.GetValue(1));
                    string group=GetString(reader.GetValue(3)).Replace(" ", "");
                    string action=GetString(reader.GetValue(4));
                    bool canBeBought=ParseBool(reader.GetValue(5));

                    try
                    {
                        if (IsStation(group) || IsUtility(group))
{
    int price=ParseCurrency(reader.GetValue(7));
    boards.Add(new BuyableBoard(positionNo, property, group, price));
}
                        else if (canBeBought)
                        {
                            var (price, baseRent, improvedRents)=ParsePropertyData(reader);
                            boards.Add(new estateBoard(
                                positionNo, property, group, 
                                price, baseRent, improvedRents
                            ));
                        }
                        else
                        {
                            boards.Add(new Board(positionNo, property, group, action, false));
                            
                        }
                    }
                    catch (Exception ex)
                    {
             
                    }
                }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"fall to load excel: {ex.Message}");
        }
        ValidateAllBoards(boards);

        return boards;
    }

    private static string GetString(object cell) => cell?.ToString().Trim() ?? "";
    
    private static int ParseInt(object cell) => 
        int.TryParse(GetString(cell), out int num) ? num : 0;

    private static bool ParseBool(object cell) => 
        GetString(cell).Equals("yes", StringComparison.OrdinalIgnoreCase);

    private static int ParseCurrency(object cell)
    {
        string value=GetString(cell);
        if (value.Contains("See notes")) return 0;
        return int.TryParse(value.Replace("£", ""), out int num) ? num : 0;
    }
   

  
    private static bool IsStation(string group) => 
        group.Equals("Station", StringComparison.OrdinalIgnoreCase);

    private static bool IsUtility(string group) => 
        group.Equals("Utilities", StringComparison.OrdinalIgnoreCase);

 

    private static (int price, int baseRent, int[] improvedRents) ParsePropertyData(IExcelDataReader reader)
    {
        int price=ParseCurrency(reader.GetValue(7));
        int baseRent=ParseCurrency(reader.GetValue(8));
        
        int[] improvedRents=new int[5];
        for (int i=0; i < 5; i++)
        {
            improvedRents[i]=ParseCurrency(reader.GetValue(10 + i));
        }

        if (price <= 0) throw new ArgumentException("price have to larger than 0");
        if (baseRent <= 0) throw new ArgumentException("base rent have to larger than 0");
        
        return (price, baseRent, improvedRents);
    }


    private static void ValidateAllBoards(List<Board> _boards)
    {
        foreach (var board in _boards)
        {
            switch (board)
            {
                case estateBoard estate:
                    if (estate.owner == null)
                        Debug.LogError($" {estate.property} has no owner");
                    if (!estate.canBeBought)
                        Debug.LogError($"{estate.property} can not be purchase");
                    break;
                

            }
        }
    }
   
  
}

