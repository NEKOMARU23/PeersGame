using System.Collections.Generic;
using UnityEngine;
using TechC.InGame.Enemy;
using TechC.InGame.Log;

namespace TechC.InGame.Map
{
    /// <summary>
    /// 生成済みマップのグリッド状態を管理するクラス
    /// </summary>
    public class MapManager
    {
        private readonly TileData[,] _tiles;
        
        // ★重要：初期値は0。MapGeneratorから確定した数をAddEnemyCountで受け取る。
        private int _currentEnemyCount = 0;

        /// <summary>縦のマス数</summary>
        public int Rows { get; }

        /// <summary>横のマス数</summary>
        public int Columns { get; }

        public MapManager(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            _tiles = new TileData[rows, columns];
        }

        /// <summary>
        /// タイルを登録する
        /// </summary>
        public void RegisterTile(TileData tileData)
        {
            var pos = tileData.GridPosition;
            _tiles[pos.y, pos.x] = tileData;
            
            // ★修正：ここでの自動カウントを廃止。
            // 生成タイミングの前後によりカウント漏れが起きるのを防ぐため。
        }

        /// <summary>
        /// 現在マップに残っている敵の総数を取得する
        /// </summary>
        public int GetEnemyCount()
        {
            return _currentEnemyCount;
        }

        /// <summary>
        /// 敵の初期数を設定、または追加する
        /// </summary>
        public void AddEnemyCount(int amount)
        {
            _currentEnemyCount += amount;
            CusLog.Log($"[MapManager] 敵カウントを更新しました。現在の総数: {_currentEnemyCount}");
        }

        public bool IsValidPosition(Vector2Int pos)
        {
            if (pos.x < 0 || pos.x >= Columns) return false;
            if (pos.y < 0 || pos.y >= Rows) return false;

            var tile = _tiles[pos.y, pos.x];
            return tile != null && tile.IsWalkable;
        }

        public TileData GetTile(Vector2Int pos)
        {
            if (!IsValidPosition(pos)) return null;
            return _tiles[pos.y, pos.x];
        }

        public EnemyDataOnTile GetEnemyAt(Vector2Int pos)
        {
            var tile = GetTile(pos);
            return tile?.EnemyObject;
        }

        public List<TileData> GetWalkableEmptyTiles(Vector2Int playerPos)
        {
            var walkableEmptyTiles = new List<TileData>();

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    var tile = _tiles[y, x];
                    if (tile == null || !tile.IsWalkable || tile.IsItem || tile.EnemyObject != null) continue;
                    if (x == playerPos.x && y == playerPos.y) continue;

                    walkableEmptyTiles.Add(tile);
                }
            }
            return walkableEmptyTiles;
        }

        /// <summary>
        /// 敵を削除し、カウントを減らす。
        /// </summary>
        public void RemoveEnemyAt(Vector2Int pos)
        {
            TileData tile = GetTile(pos);

            if (tile != null && tile.EnemyObject != null)
            {
                // ★カウントを減らす。
                _currentEnemyCount--;
                CusLog.Log($"[MapManager] 敵を削除。残り敵数: {_currentEnemyCount}");
                
                Object.Destroy(tile.EnemyObject.gameObject);
                tile.EnemyObject = null;
            }
            else
            {
                CusLog.Warning($"[MapManager] 指定座標 {pos} に削除すべき敵がいません。");
            }
        }
    }
}