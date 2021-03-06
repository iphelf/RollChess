using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
///   <para> 管理Tilemap </para>
///   <para> 使用时挂在要管理的tilemap下面 </para>
///   <para> 注意：不允许多个Manager共用一个tilemap，两者必须一对一 </para>
/// </summary>
public class TilemapManager : MonoBehaviour {

    // Tilemap本体
    [SerializeField] private Tilemap tilemap;

    // key为接受的有效Tile范围，value为对应的Tile
    private Dictionary<TileType, Tile> pallette = new Dictionary<TileType, Tile>();

    // 已填充的坐标，只读
    private HashSet<Vector2Int> cellSet = new HashSet<Vector2Int>();

    // 仅用于在Unity中初始化pallette
    [SerializeField] private List<TileType> _pallette;

    void Start() {
        // 初始化pallette
        foreach(TileType type in _pallette) {
            pallette[type] = Resources.Load<Tile>(Transform.resourceOfTileType[type]);
        }
    }

    /// <summary>
    ///   <para> 增改Tile </para>
    /// </summary>
    public void SetTile(Vector2Int position, TileType tileType) {
        // tileType必须在pallette允许范围内
        Debug.Assert(TileTypeAvailable(tileType));
        // 设置并显示
        cellSet.Add(position);
        tilemap.SetTile((Vector3Int) position, pallette[tileType]);
    }

    /// <summary>
    ///   <para> 获取该坐标的TileType </para>
    /// </summary>
    public TileType GetTile(Vector2Int position) {
        Tile tile = tilemap.GetTile<Tile>((Vector3Int) position);
        if(tile is null)
            return TileType.None;
        foreach(KeyValuePair<TileType, Tile> kvp in pallette) {
            if(kvp.Value == tile)
                return kvp.Key;
        }
        return TileType.None;
    }

    /// <summary>
    ///   <para> 移除该坐标的Tile </para>
    /// </summary>
    public void RemoveTile(Vector2Int position) {
        tilemap.SetTile((Vector3Int) position, null);
        cellSet.Remove(position); 
    }

    /// <summary>
    ///   <para> 世界坐标转换Tilemap坐标 </para>
    /// </summary>
    public Vector2Int WorldToCell(Vector3 pos) {
        Vector3Int vector = tilemap.WorldToCell(pos);
        return new Vector2Int(vector.x, vector.y);
    }

    /// <summary>
    ///   <para> Tilemap坐标转换世界坐标 </para>
    /// </summary>
    public Vector3 CellToWorld(Vector2Int pos) {
        return tilemap.CellToWorld(new Vector3Int(pos.x, pos.y, 0));
    }

    /// <summary>
    ///   <para> Tilemap坐标转换local坐标 </para>
    /// </summary>
    public Vector3 CellToLocal(Vector3Int pos) {
        return tilemap.CellToLocal(pos);
    }

    /// <summary>
    ///   <para> 格子大小 </para>
    /// </summary>
    public Vector3 CellSize() {
        return tilemap.cellSize;
    }

    /// <summary>
    ///   <para> 鼠标当前所在格子坐标 </para>
    /// </summary>
    public Vector2Int CursorPointingCell() {
        // 获取屏幕坐标，转换为世界坐标，再转换为Tilemap的坐标
        Vector3 cursorPositionScreen = CursorMonitor.MouseScreenPositionNow;
        Vector3 cursorPositionWorld = ToolResource.cameraController.ScreenToWorld(cursorPositionScreen);
        return (Vector2Int)tilemap.WorldToCell(cursorPositionWorld);
    }

    /// <summary>
    ///   <para> 判断此TileType是否合法 </para>
    /// </summary>
    public bool TileTypeAvailable(TileType tileType) {
        return pallette.ContainsKey(tileType);
    }

    /// <summary>
    ///   <para> 已填充的坐标，只读 </para>
    /// </summary>
    public HashSet<Vector2Int> CellSet {
        get{return cellSet;}
    }
}