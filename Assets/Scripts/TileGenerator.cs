using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TilePrefabConfig
    {
        public string tileType;         
        public GameObject tilePrefab;    
    }

    [SerializeField] private List<TilePrefabConfig> _tilePrefabs = new List<TilePrefabConfig>();
    private Dictionary<string, GameObject> _tileDict = new Dictionary<string, GameObject>(); 

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
                Debug.Log($"成功生成 tile prefab，索引为: {key}");
            }
            else
            {
                Debug.LogWarning($"重复的格子类型: {key}");
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
            tile.name = $"Tile_{board.positionNo}_{mapkey}";
            tile.transform.SetParent(this.transform);
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
