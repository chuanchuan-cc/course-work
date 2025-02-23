using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class PlayerData: ScriptableObject
{
    public int money;
    public int positionNo;
    public int freezeTurn;
    public bool isBankrupt;
    public List<string> assetsList;
    public int circle;
    public int assetsWorth;
    



    
}
