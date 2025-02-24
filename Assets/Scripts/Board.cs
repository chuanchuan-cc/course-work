using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
    public Player owner;
    public estateBoard(int positionNo, string property, string action, int price,int baseRent, int[] improvedRents):base(positionNo,property,"estate",action,true){
        this.improvedLevel=0;
        this.price=price;
        this.baseRent = baseRent;
        this.improvedRents = improvedRents;
        this.owner = RunGame.bank;
        this.rent=baseRent+improvedRents[improvedLevel];
    

    }
    [Serializable]
public class StationBoard : Board {
    public int price;
    public Player owner;
    

    public StationBoard(int positionNo, string property, int price) 
    : base(positionNo, property, "Station", "", true) {
        this.price = price;
        this.owner = RunGame.bank;
    }
}

[Serializable]
public class UtilityBoard : Board {
    public int price;
    public Player owner;

    public UtilityBoard(int positionNo, string property, int price) 
        : base(positionNo, property, "Utilities","", true) {
        this.price = price;
        this.owner = RunGame.bank;
    }
}
}
public static class BoardLoader
{
    public static List<Board> LoadBoards(string csvPath)
    {
        List<Board> boards = new List<Board>();

        try
        {
            string[] lines = File.ReadAllLines(csvPath);
            
            for (int i = 4; i < lines.Length; i++) 
            {
                if (string.IsNullOrWhiteSpace(lines[i])) continue;

                string[] cols = lines[i].Split(',');
                if (cols.Length < 19) continue;  

                int positionNo = SafeParseInt(cols[0]);
                string property = cols[1].Trim();
                string group = cols[3].Trim();
                string action = cols[4].Trim();
                bool canBeBought = cols[5].Trim().Equals("Yes", StringComparison.OrdinalIgnoreCase);

                if (group.Equals("Station", StringComparison.OrdinalIgnoreCase))
                {
                    int price = SafeParseCurrency(cols[7]);
                    boards.Add(new StationBoard(positionNo, property, price));
                }
                else if (group.Equals("Utilities", StringComparison.OrdinalIgnoreCase))
                {
                    int price = SafeParseCurrency(cols[7]);
                    boards.Add(new UtilityBoard(positionNo, property, price));
                }
                else if (canBeBought)
                {
                    int price = SafeParseCurrency(cols[7]);
                    int baseRent = SafeParseCurrency(cols[8]);
                    
                    int[] improvedRents = new int[5];
                    for (int j = 0; j < 5; j++)
                    {
                        improvedRents[j] = SafeParseCurrency(cols[10 + j]);
                    }

                    boards.Add(new EstateBoard(
                        positionNo, property, group, 
                        price, baseRent, improvedRents
                    ));
                }
                else
                {
                    boards.Add(new Board(
                        positionNo, property, group, action, false
                    ));
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"加载棋盘数据失败: {ex.Message}");
        }

        return boards;
    }

    private static int SafeParseInt(string input)
    {
        return int.TryParse(input?.Trim(), out int result) ? result : 0;
    }

    private static int SafeParseCurrency(string input)
    {
        return int.TryParse(input?.Replace("£", "").Trim(), out int result) ? result : 0;
    }
}

public class BoardManager : MonoBehaviour
{
    private List<Board> gameBoards;

    void Start()
    {
        LoadBoardData();
    }

    void LoadBoardData()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "BoardData.csv");
        gameBoards = BoardLoader.LoadBoards(path);

        foreach (Board board in gameBoards)
        {
            if (board is EstateBoard estate)
            {
                Debug.Log($"地产 [{estate.positionNo}] {estate.property} " +
                         $"价格: £{estate.price} 基础租金: £{estate.baseRent}");
            }
            else if (board is StationBoard station)
            {
                Debug.Log($"车站 [{station.positionNo}] {station.property} " +
                         $"价格: £{station.price}");
            }
        }
    }
}