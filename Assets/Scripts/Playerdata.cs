using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;


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
    public List<Board> assetsList=new List<Board>();
    public int circle=1;
    public int assetsWorth;
    public List<string> assetsName=new List<string>();


    public string GetName(){
        return this.name;
    }
    public PlayerData DeepCopy()
    {

        string json = JsonConvert.SerializeObject(this, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        return JsonConvert.DeserializeObject<PlayerData>(json, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
    }
    



    
}
public interface IOwner{
    string GetName();
}
