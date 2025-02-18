public class Board{

    public string property;
    public string group;
    public bool canBeBought;
    public Board(string property, string group, bool canBeBought){
        this.canBeBought=canBeBought;
        this.group=group;
        this.property=property;
    }
    public Board(string property, bool canBeBought){
        this.canBeBought=canBeBought;
        this.property=property;
    }


}
public class estateBoard:Board{
    public int price;
    public int rent;
    public Player owner;
    public estateBoard(string property, string group, bool canBeBought,int price,int rent):base(property,group,canBeBought){
        this.price=price;
        this.rent=rent;
        owner=RunGame.bank;
    }
    
}
