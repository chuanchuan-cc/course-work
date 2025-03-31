using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TileGenerator : MonoBehaviour
{
    
   

    [SerializeField] private List<TilePrefabConfig> _tilePrefabs = new List<TilePrefabConfig>();
    private Dictionary<string, GameObject> _tileDict = new Dictionary<string, GameObject>(); 
    private List<GameObject> tileList=new List<GameObject>();

    void Start()
    {
        InitializeTileDictionary();
    }


    public void InitializeTileDictionary()
    {
        foreach (var config in _tilePrefabs)
        {
            string key = config.tileType.ToLower().Replace(" ", "");
            if (!_tileDict.ContainsKey(key))
            {
                _tileDict.Add(key, config.tilePrefab);

            }

        }
    }


    public void GenerateMapFromList(List<Board> maplist)
    {
        if (maplist == null || maplist.Count == 0)
        {
            Debug.LogError("maplist 为空，无法生成地图！");
            return;
        }

        foreach (Board board in maplist)
        {
            string mapkey = string.IsNullOrEmpty(board.group) ?
                            board.property.ToLower().Replace(" ", "") :
                            board.group.ToLower().Replace(" ", "");

            if (!_tileDict.TryGetValue(mapkey, out GameObject prefab))
            {
                Debug.LogError($"未找到匹配的 tile prefab，跳过生成: {board.property}，资源索引为 {mapkey}");
                continue;
            }
           
            
            Vector3Int tilePosition = BoardGetPosition(board.positionNo);
            Vector3 worldPosition = new Vector3(tilePosition.x + 0.5f, tilePosition.y + 0.5f, 0f); 

            GameObject tile = Instantiate(prefab, worldPosition, Quaternion.identity);
            tileList.Add(tile);
            tile.name = $"Tile_{board.positionNo}_{mapkey}";
            tile.transform.SetParent(this.transform);
            if(board.canBeBought){
                
           updateTile(board);
            }


            
            }
            
        
    
    }

public void updateTile(Board board){
    Debug.Log($"更新格子 {board.positionNo}");
            GameObject tile=tileList[board.positionNo];
                
            estatePrefabConfig config = tile.GetComponent<estatePrefabConfig>();
            if (config!=null){
                estateBoard eBoard= board as estateBoard;
                if (eBoard!=null)config.updateConfigEstate(eBoard);
                else {
                    BuyableBoard bBoard= board as BuyableBoard;
                    if(bBoard!=null)
                    config.updateConfigBuyable(bBoard);
                    
                }                
            
            }
                

}

    private Vector3Int BoardGetPosition(int no)
    {
        if (no == 0) return new Vector3Int(7, -3, 0);
        else if (no <= 15) return new Vector3Int(7 - no, -3, 0);
        else if (no <= 20) return new Vector3Int(-8, no - 15 - 3, 0);
        else if (no <= 35) return new Vector3Int(-8 + no - 20, 2, 0);
        else return new Vector3Int(7, 2 - (no - 35), 0);
    }
}
