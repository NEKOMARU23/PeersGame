using UnityEngine;
using TechC.InGame.Log;

namespace TechC.InGame.Map
{
    /// <summary>
    /// MapDataSOを元にタイルを生成し、MapManagerを構築するクラス
    /// InGameManagerにアタッチして使用する
    /// </summary>
    public class MapGenerator : MonoBehaviour
    {
        private const float TileHeight = 0f;

        [SerializeField] private MapDataSO _mapData;
        [SerializeField] private Transform _tileParent;

        /// <summary>
        /// マップを生成してMapManagerを返す
        /// </summary>
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
                Destroy(_tileParent.GetChild(i).gameObject);

            var mapManager = new MapManager(_mapData.Rows, _mapData.Columns);

            float offsetX = (_mapData.Columns - 1) * _mapData.TileSpacing / 2f;
            float offsetZ = (_mapData.Rows - 1) * _mapData.TileSpacing / 2f;

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

            return mapManager;
        }
    }
}
