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

            var mapManager = new MapManager(_mapData.Rows, _mapData.Columns);

            for (int row = 0; row < _mapData.Rows; row++)
            {
                for (int col = 0; col < _mapData.Columns; col++)
                {
                    var gridPos = new Vector2Int(col, row);
                    var worldPos = new Vector3(col, TileHeight, row);

                    var tileObject = Instantiate(
                        _mapData.TilePrefab,
                        worldPos,
                        Quaternion.identity,
                        _mapData.TileParent
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
