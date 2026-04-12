using UnityEngine;

namespace TechC.InGame.Map
{
    /// <summary>
    /// タイル1枚分の情報を保持するクラス
    /// </summary>
    public class TileData
    {
        /// <summary>グリッド上の座標（列, 行）</summary>
        public Vector2Int GridPosition { get; }

        /// <summary>実際に生成されたGameObject</summary>
        public GameObject TileObject { get; }

        /// <summary>歩行可能かどうか（将来の拡張用）</summary>
        public bool IsWalkable { get; set; } = true;

        /// <summary>アイテムがついているかどうか</summary>
        public bool IsItem { get; set; } = false;//

        /// <summary>タイルに付随したアイテムのGameObject</summary>
        public GameObject ItemObject { get; set; }//

        /// <summary>グリッド座標と対応するGameObjectを指定して初期化する</summary>
        public TileData(Vector2Int gridPosition, GameObject tileObject)
        {
            GridPosition = gridPosition;
            TileObject = tileObject;
        }
    }
}
