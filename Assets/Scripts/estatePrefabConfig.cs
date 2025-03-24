using UnityEngine;
using TMPro;
public class estatePrefabConfig :  TilePrefabConfig{
        public TextMeshPro property;
        public TextMeshPro price;
        public TextMeshPro rent;
        public TextMeshPro owner;
        public TextMeshPro improvedLevel;




    public void updateConfigEstate(estateBoard board){
        property.text=$"property : {board.property.ToString()}";
        price.text=$"price : {board.price.ToString()}";
        if(board.rent!=null){
        rent.text=$"rent : {board.rent.ToString()}";}
        owner.text=$"owner : {board.owner.GetName()}";
        improvedLevel.text=$"improve level : {board.improvedLevel.ToString()}";

    }
        public void updateConfigBuyable(BuyableBoard board){
        property.text=$"property : {board.property.ToString()}";
        price.text=$"price : {board.price.ToString()}";
        if(board.rent!=null){
        rent.text=$"rent : {board.rent.ToString()}";}
        owner.text=$"owner : {board.owner.GetName()}";

    }
    }