using UnityEngine;
using UnityEngine.InputSystem;
using TechC.InGame.Core;
using TechC.InGame.Log;
using System;//

namespace TechC.InGame.Player
{
    /// <summary>
    /// WASDによるグリッド移動のテスト用クラス
    /// InGameManager.I経由でMapManagerを参照し、有効なマスにのみ移動する
    /// </summary>
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Vector2Int _startGridPos;

        private Vector2Int _currentGridPos;

        private float _placementY;

        /// <summary>タイルに到達した時のイベント</summary>
        public static event Action<Map.TileData> OnTileReached;//

        private void Start()
        {
            var mapManager = InGameManager.I?.MapManager;
            if (mapManager == null)
            {
                CusLog.Warning("[TestPlayerMover] MapManager が取得できませんでした。");
                return;
            }

            if (!mapManager.IsValidPosition(_startGridPos))
            {
                CusLog.Warning($"[TestPlayerMover] 開始グリッド座標 {_startGridPos} が無効です。");
                return;
            }

            _currentGridPos = _startGridPos;
            var tile = mapManager.GetTile(_currentGridPos);

            CachePlacementY(tile);

            var tilePos = tile.TileObject.transform.position;
            transform.position = new Vector3(tilePos.x, _placementY, tilePos.z);
        }

        /// <summary>タイル上面とプレイヤー底部からY座標を計算してキャッシュする</summary>
        private void CachePlacementY(Map.TileData tile)
        {
            var tileRenderer = tile.TileObject.GetComponent<Renderer>();
            if (tileRenderer == null)
            {
                CusLog.Error("Rendererがnullです");
                return;
            }
            float tileTopY = tileRenderer.bounds.max.y;

            var playerRenderer = GetComponent<Renderer>();
            if (playerRenderer == null)
            {
                CusLog.Error("Rendererがnullです");
                return;
            }
            float playerBottomOffset = transform.position.y - playerRenderer.bounds.min.y;

            _placementY = tileTopY + playerBottomOffset;
        }

        private void Update()
        {
            var input = GetMoveInput();
            if (input == Vector2Int.zero) return;

            TryMove(input);
        }

        private Vector2Int GetMoveInput()
        {
            var kb = Keyboard.current;
            if (kb == null) return Vector2Int.zero;

            if (kb.wKey.wasPressedThisFrame) return Vector2Int.up;
            if (kb.sKey.wasPressedThisFrame) return Vector2Int.down;
            if (kb.aKey.wasPressedThisFrame) return Vector2Int.left;
            if (kb.dKey.wasPressedThisFrame) return Vector2Int.right;
            return Vector2Int.zero;
        }

        private void TryMove(Vector2Int direction)
        {
            var mapManager = InGameManager.I?.MapManager;
            if (mapManager == null)
            {
                CusLog.Warning("[TestPlayerMover] MapManager が取得できませんでした。");
                return;
            }

            var nextPos = _currentGridPos + direction;
            if (!mapManager.IsValidPosition(nextPos)) return;

            _currentGridPos = nextPos;
            var tile = mapManager.GetTile(_currentGridPos);
            var tilePos = tile.TileObject.transform.position;
            transform.position = new Vector3(tilePos.x, _placementY, tilePos.z);
            
            InGameManager.I.ScoreManager.AddMoveSuccessScore();
            
            // タイルに到達したことを通知
            OnTileReached?.Invoke(tile);//
        }
    }
}
