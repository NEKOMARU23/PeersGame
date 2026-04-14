using System.Collections.Generic;
using UnityEngine;
using TechC.InGame.Enemy;

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

        /// <summary>
        /// 指定座標に敵がいるかどうかを返す
        /// </summary>
        public EnemyDataOnTile GetEnemyAt(Vector2Int pos)
        {
            var tile = GetTile(pos);
            return tile?.EnemyObject;
        }

        /// <summary>
        /// プレイヤー、アイテム、敵のいずれも存在しない「完全な空きタイル」のリストを取得する
        /// </summary>
        /// <param name="playerPos">除外したいプレイヤーの現在座標</param>
        public List<TileData> GetWalkableEmptyTiles(Vector2Int playerPos)
        {
            var walkableEmptyTiles = new List<TileData>();

            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    var tile = _tiles[y, x];

                    // 基本チェック
                    if (tile == null) continue;
                    if (!tile.IsWalkable) continue;

                    // オブジェクト重複チェック
                    if (tile.IsItem) continue;           // アイテムがある場合はスキップ
                    if (tile.EnemyObject != null) continue; // 敵がある場合はスキップ

                    // プレイヤー座標チェック
                    if (x == playerPos.x && y == playerPos.y) continue;

                    walkableEmptyTiles.Add(tile);
                }
            }

            return walkableEmptyTiles;
        }

        /// <summary>
        /// 指定座標の敵を削除し、タイルの情報をクリアする
        /// </summary>
        /// <param name="pos">消去したい敵のグリッド座標</param>
        public void RemoveEnemyAt(Vector2Int pos)
        {
            TileData tile = GetTile(pos);

            if (tile != null && tile.EnemyObject != null)
            {
                Object.Destroy(tile.EnemyObject.gameObject);

                tile.EnemyObject = null;
            }
        }
    }
}