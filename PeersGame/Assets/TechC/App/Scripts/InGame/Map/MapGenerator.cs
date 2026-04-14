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
            if (_mapData == null)
            {
                CusLog.Error("[MapGenerator] MapDataSO がアサインされていません。");
                return null;
            }

            if (_mapData.TilePrefab == null)
            {
                CusLog.Error("[MapGenerator] TilePrefab がアサインされていません。");
                return null;
            }

            if (_tileParent == null)
            {
                CusLog.Error("[MapGenerator] TileParent がアサインされていません。");
                return null;
            }

            for (int i = _tileParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_tileParent.GetChild(i).gameObject);
            }

            var mapManager = new MapManager(_mapData.Rows, _mapData.Columns);

            float offsetX = (_mapData.Columns - 1) * _mapData.TileSpacing / HalfDivider;
            float offsetZ = (_mapData.Rows - 1) * _mapData.TileSpacing / HalfDivider;

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

                    var tileObject = Instantiate(
                        _mapData.TilePrefab,
                        worldPos,
                        Quaternion.identity,
                        _tileParent
                    );
                    tileObject.name = $"Tile({col},{row})";

                    var tileData = new TileData(gridPos, tileObject);
                    mapManager.RegisterTile(tileData);
                }
            }

            if (_enemySpawner != null)
            {
                _enemySpawner.SpawnEnemies(mapManager, _enemySpawnCount, _playerInitialGridPos);
            }
            else
            {
                CusLog.Warning("[MapGenerator] EnemySpawner がアサインされていないため、敵を生成できませんでした。");
            }

            return mapManager;
        }
    }
}