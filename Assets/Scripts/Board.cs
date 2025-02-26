using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using System.IO;
using ExcelDataReader;


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
        this.action = action;
        this.property=property;
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
    public IOwner owner;
    public estateBoard(int positionNo, string property, string group, 
                      int price, int baseRent, int[] improvedRents) 
        : base(positionNo, property, group, "", true)
    {
        ValidateConstruction(group, price, baseRent, improvedRents);

        this.improvedLevel=0;
        this.price=price;
        this.baseRent = baseRent;
        this.improvedRents = improvedRents;
        this.owner = RunGame.bank;
        this.rent=baseRent+improvedRents[improvedLevel];
        this.isMortgage=false;
    

    }
      private void ValidateConstruction(string group, int price, int baseRent, int[] improvedRents)
    {
        if (!canBeBought)
            throw new ArgumentException("Estate boards must be buyable");
        
        if (improvedRents.Length != 5)
            throw new ArgumentException("Invalid improved rents array length");
    }
}
    [Serializable]
public class StationBoard : Board {
    public int price;
    public IOwner owner;
    

    public StationBoard(int positionNo, string property, int price) 
    : base(positionNo, property, "Station", "", true) {
        this.price = price;
        this.owner = RunGame.bank;
    }
}

[Serializable]
public class UtilityBoard : Board {
    public int price;
    public IOwner owner;

    public UtilityBoard(int positionNo, string property, int price) 
        : base(positionNo, property, "Utilities","", true) {
        this.price = price;
        this.owner = RunGame.bank;
    }
}

public static class BoardLoader
{
    public static List<Board> LoadBoards(string excelPath)
    {
        List<Board> boards = new List<Board>();

        try
        {
            
            using (var stream = File.Open(excelPath, FileMode.Open, FileAccess.Read))
            using (var reader = ExcelReaderFactory.CreateReader(stream))
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
                    
                    
                    int positionNo = ParseInt(reader.GetValue(0))-1;
                    string property = GetString(reader.GetValue(1));
                    string group = GetString(reader.GetValue(3));
                    string action = GetString(reader.GetValue(4));
                    bool canBeBought = ParseBool(reader.GetValue(5));

                    try
                    {
                        if (IsStation(group))
                        {
                            int price = ParseCurrency(reader.GetValue(7));
                            ValidateStation(reader);
                            boards.Add(new StationBoard(positionNo, property, price));
                        }
                        else if (IsUtility(group))
                        {
                            int price = ParseCurrency(reader.GetValue(7));
                            boards.Add(new UtilityBoard(positionNo, property, price));
                        }
                        else if (canBeBought)
                        {
                            var (price, baseRent, improvedRents) = ParsePropertyData(reader);
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
                        Debug.LogError($"fail to load data in row {rowIdx}");
                    }
                }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Excel加载失败: {ex.Message}");
        }
        ValidateAllBoards(boards);

        return boards;
    }

    #region 
    private static string GetString(object cell) => cell?.ToString().Trim() ?? "";
    
    private static int ParseInt(object cell) => 
        int.TryParse(GetString(cell), out int num) ? num : 0;

    private static bool ParseBool(object cell) => 
        GetString(cell).Equals("yes", StringComparison.OrdinalIgnoreCase);

    private static int ParseCurrency(object cell)
    {
        string value = GetString(cell);
        if (value.Contains("See notes")) return 0;
        return int.TryParse(value.Replace("£", ""), out int num) ? num : 0;
    }
    #endregion

    #region 
    private static bool IsStation(string group) => 
        group.Equals("Station", StringComparison.OrdinalIgnoreCase);

    private static bool IsUtility(string group) => 
        group.Equals("Utilities", StringComparison.OrdinalIgnoreCase);

    private static void ValidateStation(IExcelDataReader reader)
    {
        if (ParseCurrency(reader.GetValue(8)) != 0)
            Debug.LogWarning("车站不应有基础租金数值");
    }

    private static (int price, int baseRent, int[] improvedRents) ParsePropertyData(IExcelDataReader reader)
    {
        int price = ParseCurrency(reader.GetValue(7));
        int baseRent = ParseCurrency(reader.GetValue(8));
        
        int[] improvedRents = new int[5];
        for (int i = 0; i < 5; i++)
        {
            improvedRents[i] = ParseCurrency(reader.GetValue(10 + i));
        }

        if (price <= 0) throw new ArgumentException("价格必须大于0");
        if (baseRent <= 0) throw new ArgumentException("基础租金必须大于0");
        
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
                        Debug.LogError($"地产 {estate.property} 缺少所有者");
                    if (!estate.canBeBought)
                        Debug.LogError($"地产 {estate.property} 标记不可购买");
                    break;
                
                case StationBoard station:
                    if (station.price <= 0)
                        Debug.LogError($"车站 {station.property} 价格无效");
                    break;
            }
        }
    }
    #endregion
}

