using UnityEngine;

namespace TechC.InGame.Map
{
    /// <summary>
    /// 生成済みマップのグリッド状態を管理するクラス
    /// </summary>
    public class MapManager
    {
        private readonly TileData[,] _tiles;

        /// <summary>縦のマス数</summary>
        public int Rows { get; }

        /// <summary>横のマス数</summary>
        public int Columns { get; }

        /// <summary>マップのサイズを指定して初期化する</summary>
        public MapManager(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _tiles = new TileData[rows, columns];
        }

        /// <summary>
        /// タイルを登録する（MapGeneratorから呼ばれる）
        /// </summary>
        public void RegisterTile(TileData tileData)
        {
            var pos = tileData.GridPosition;
            _tiles[pos.y, pos.x] = tileData;
        }

        /// <summary>
        /// 指定座標にタイルが存在し、かつ歩行可能かどうかを返す
        /// </summary>
        public bool IsValidPosition(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Columns) return false;
            if (pos.y < 0 || pos.y >= Rows) return false;

            var tile = _tiles[pos.y, pos.x];
            return tile != null && tile.IsWalkable;
        }

        /// <summary>
        /// 指定座標のTileDataを取得する
        /// </summary>
        public TileData GetTile(Vector2Int pos)
        {
            if (!IsValidPosition(pos)) return null;
            return _tiles[pos.y, pos.x];
        }
    }
}
