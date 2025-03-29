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
        if(board.rent!=0){
        rent.text=$"rent : {board.rent.ToString()}";}
        else rent.text="unknown";
        owner.text=$"owner : {board.owner.GetName()}";
        improvedLevel.text=$"building level : {board.improvedLevel.ToString()}";

    }
        public void updateConfigBuyable(BuyableBoard board){
        property.text=$"property : {board.property.ToString()}";
        price.text=$"price : {board.price.ToString()}";
        if(board.rent!=0){
        rent.text=$"rent : {board.rent.ToString()}";}
        else rent.text="unknown";
        owner.text=$"owner : {board.owner.GetName()}";

    }
    }