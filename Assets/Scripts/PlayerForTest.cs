using System.Collections.Generic;
public class Player{
    public int money;
    public int position;
    public int freezeTurn;
    public string name;
    public bool isBankrupt;
    public List<string> assetsList;
    public int circle;
    public int assetsWorth;
    public Player(string name){
        this.name=name;
        position=0;
        freezeTurn=0;
        money=1500;
        isBankrupt=false;
        assetsList= new List<string>();
        circle=0;
        assetsWorth=1500;

    }
}