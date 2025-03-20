using System.Collections.Generic;
using UnityEngine;

public class TileGenerator : MonoBehaviour
{
    [System.Serializable]
    public class TilePrefabConfig
    {
        public string tileType;      // 格子类型（与maplist中的类型字段完全匹配）
        public GameObject prefab;    // 对应的预制体
    }

    [Header("预制体配置")]
    [SerializeField] private List<TilePrefabConfig> _tilePrefabs = new List<TilePrefabConfig>(); // 预制体库
    [SerializeField] private GameObject _defaultPrefab; // 默认预制体（用于未知类型）
    public List<Board> maplist;



    private Dictionary<string, GameObject> _prefabDict = new Dictionary<string, GameObject>();

    void Start()
    {
        InitializePrefabDictionary();
        GenerateMapFromList();
    }

    // 初始化预制体字典（类型 -> 预制体）
    private void InitializePrefabDictionary()
    {
        foreach (var config in _tilePrefabs)
        {
            if (!_prefabDict.ContainsKey(config.tileType))
            {
                _prefabDict.Add(config.tileType, config.prefab);
            }
            else
            {
                Debug.LogWarning($"重复的格子类型: {config.tileType}");
            }
        }
    }

    // 根据maplist生成地图
    private void GenerateMapFromList()
    {
        foreach (Board board in maplist)
        {
            // 1. 获取预制体
            GameObject prefab = GetPrefabByType(board.group);

            // 2. 实例化并设置位置
            GameObject tile = Instantiate(prefab, board.position, Quaternion.identity, transform);
            tile.name = $"{data.tileType}_{data.index}";

            // 3. 可选：附加数据到格子对象
            TileLogic logic = tile.GetComponent<TileLogic>();
            if (logic != null)
            {
                logic.Initialize(data);
            }
        }
    }

    // 根据类型获取预制体（未知类型返回默认）
    private GameObject GetPrefabByType(string type)
    {
        if (_prefabDict.TryGetValue(type, out GameObject prefab))
        {
            return prefab;
        }
        Debug.LogWarning($"未知格子类型: {type}，使用默认预制体");
        return _defaultPrefab;
    }
}

