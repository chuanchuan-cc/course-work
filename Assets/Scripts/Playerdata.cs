using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData: ScriptableObject,IOwner
{
    public string name;
    public int freeJail;
    public int money;
    public int positionNo;
    public int freezeTurn;
    public bool isBankrupt;
    public List<string> assetsList;
    public int circle;
    public int assetsWorth;

    public string GetName(){
        return name;
    }
    



    
}
public interface IOwner{
    string GetName();
}