using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TilePrefabConfig
    {
        public string tileType;  // 格子类型（与 maplist 中的类型字段匹配）
        public TileBase tile;    // 对应的 Tile 资源
    }

    [SerializeField] private List<TilePrefabConfig> _tilePrefabs = new List<TilePrefabConfig>(); // Tile 资源库
    private Dictionary<string, TileBase> _tileDict = new Dictionary<string, TileBase>(); // Tile 映射表
    private Tilemap tilemap;

    void Awake()
    {
        // 找到场景中的 Tilemap
        tilemap = GameObject.Find("Tilemap").GetComponent<Tilemap>();
        if (tilemap == null)
        {
            Debug.LogError("Tilemap 组件未找到！请检查场景中是否有 Tilemap 对象");
        }
    }

    void Start()
    {
        InitializeTileDictionary();
    }

    // 初始化 Tile 资源字典（类型 -> Tile）
    public void InitializeTileDictionary()
    {
        foreach (var config in _tilePrefabs)
        {
            if (!_tileDict.ContainsKey(config.tileType))
            {
                _tileDict.Add(config.tileType, config.tile);
                Debug.Log($"成功生成tile资源，索引为:{config.tileType}");
            }
            else
            {
                Debug.LogWarning($"重复的格子类型: {config.tileType}");
            }
        }
    }

    // 生成地图
    public void GenerateMapFromList(List<Board> maplist)
    {
        if (maplist == null || maplist.Count == 0)
        {
            Debug.LogError("maplist 为空，无法生成地图！");
            return;
        }

        foreach (Board board in maplist)
        {
            TileBase tile;
            string mapkey;
            // 1. 获取 Tile 资源
            if (board.group == "") { mapkey=board.property.ToLower().Replace(" ", ""); }
            else {mapkey=board.group.ToLower().Replace(" ", ""); }
            tile = GetTileByType(mapkey);

            if (tile == null)
            {
                Debug.LogError($"未找到匹配的 Tile 资源，跳过生成: {board.property}，资源索引为{mapkey}");
                continue;
            }

            // 2. 计算 Tilemap 坐标
            Vector3Int tilePosition = BoardGetPosition(board.positionNo);

            // 3. 在 Tilemap 上放置 Tile
            tilemap.SetTile(tilePosition, tile);
        }
    }

    // 根据类型获取 Tile 资源（未知类型返回 null）
    private TileBase GetTileByType(string type)
    {
        if (_tileDict.TryGetValue(type, out TileBase tile))
        {
            return tile;
        }
        Debug.LogWarning($"未知格子类型: {type}，未设置 Tile");
        return null;
    }

    // 将 Board 位置编号转换为 Tilemap 坐标
    private Vector3Int BoardGetPosition(int no)
    {
        if (no == 0) return new Vector3Int(7, -3, 0);
        else if (no <= 15) return new Vector3Int(7 - no, -3, 0);
        else if (no <= 20) return new Vector3Int(-8, no - 15 - 3, 0);
        else if (no <= 35) return new Vector3Int(-8 + no - 20, 2, 0);
        else return new Vector3Int(7, 2 - (no - 35), 0);
    }
}
