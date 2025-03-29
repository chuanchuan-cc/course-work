using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData: IOwner
{
    public bool isAI;
    public string name;
    public int freeJail;
    public int money;
    public int positionNo;
    public int freezeTurn;
    public bool isBankrupt;
    public List<Board> assetsList;
    public int circle;
    public int assetsWorth;

    public string GetName(){
        return this.name;
    }
    



    
}
public interface IOwner{
    string GetName();
}
