using UnityEngine;
using TechC.InGame.Log;
using TechC.InGame.Enemy;

namespace TechC.InGame.Map
{
    public class MapGenerator : MonoBehaviour
    {
        private const float TileHeight = 0f;
        private const float HalfDivider = 2f;

        [Header("References")]
        [SerializeField] private MapDataSO _mapData;
        [SerializeField] private Transform _tileParent;
        [SerializeField] private EnemySpawner _enemySpawner;

        [Header("Spawn Settings")]
        [SerializeField] private int _enemySpawnCount = 3;
        [SerializeField] private Vector2Int _playerInitialGridPos = Vector2Int.zero;

        public MapManager Generate()
        {
            if (_mapData == null || _mapData.TilePrefab == null || _tileParent == null)
            {
                CusLog.Error("[MapGenerator] 必要な参照がアサインされていません。");
                return null;
            }

            // 既存タイルのクリア
            for (int i = _tileParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_tileParent.GetChild(i).gameObject);
            }

            var mapManager = new MapManager(_mapData.Rows, _mapData.Columns);

            float offsetX = (_mapData.Columns - 1) * _mapData.TileSpacing / HalfDivider;
            float offsetZ = (_mapData.Rows - 1) * _mapData.TileSpacing / HalfDivider;

            // タイルの生成
            for (int row = 0; row < _mapData.Rows; row++)
            {
                for (int col = 0; col < _mapData.Columns; col++)
                {
                    var gridPos = new Vector2Int(col, row);
                    var worldPos = new Vector3(
                        col * _mapData.TileSpacing - offsetX,
                        TileHeight,
                        row * _mapData.TileSpacing - offsetZ
                    );

                    var tileObject = Instantiate(_mapData.TilePrefab, worldPos, Quaternion.identity, _tileParent);
                    tileObject.name = $"Tile({col},{row})";

                    var tileData = new TileData(gridPos, tileObject);
                    mapManager.RegisterTile(tileData);
                }
            }

            // 敵の生成
            if (_enemySpawner != null)
            {
                // ★修正ポイント：Spawnerに「実際に生成した数」を返してもらうようにする
                int actualSpawnedCount = _enemySpawner.SpawnEnemies(mapManager, _enemySpawnCount, _playerInitialGridPos);

                // Spawnerから返ってきた確実な数をMapManagerにセット
                mapManager.AddEnemyCount(actualSpawnedCount);

                CusLog.Log($"[MapGenerator] 生成完了。MapManager登録数: {mapManager.GetEnemyCount()}");
            }

            return mapManager;
        }
    }
}