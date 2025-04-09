using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Collections;

public class TileGenerator : MonoBehaviour
{
    
   

    [SerializeField] private List<TilePrefabConfig> _tilePrefabs = new List<TilePrefabConfig>();
    private Dictionary<string, GameObject> _tileDict = new Dictionary<string, GameObject>(); 
    private List<GameObject> tileList=new List<GameObject>();
    public bool isAnimationBoard=false;

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
            Debug.LogError("maplist is null");
            return;
        }

        foreach (Board board in maplist)
        {
            string mapkey = string.IsNullOrEmpty(board.group) ?
                            board.property.ToLower().Replace(" ", "") :
                            board.group.ToLower().Replace(" ", "");

            if (!_tileDict.TryGetValue(mapkey, out GameObject prefab))
            {
                Debug.LogError($"can find match tile prefab, skip: {board.property}, which key is {mapkey}");
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
    Debug.Log($"update tile {board.positionNo}");
    if(tileList!=null && tileList.Count!=0){
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
            
            }}
                

}
public IEnumerator BoardAnimation(int no){
    isAnimationBoard=true;
    GameObject tile = tileList[no];
    float total = 0.15f;
    float time=0f;
    Vector3 iniVect=tile.transform.position;
    Vector3 target= new Vector3(iniVect.x,iniVect.y-0.04f,iniVect.z);
    Vector3 velocity = Vector3.zero;
    while(time<total){
        tile.transform.position = Vector3.SmoothDamp(tile.transform.position, target, ref velocity, total - time);
        time+=Time.deltaTime;
         yield return  null;
    } 

    time=0f;
    velocity = Vector3.zero;
       while(time<total){
        tile.transform.position = Vector3.SmoothDamp(tile.transform.position, iniVect, ref velocity, total - time);
        time+=Time.deltaTime;
        yield return  null;
    }
    tile.transform.position=iniVect;
    isAnimationBoard=false;




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
