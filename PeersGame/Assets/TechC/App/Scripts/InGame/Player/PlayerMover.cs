using UnityEngine;
using UnityEngine.InputSystem;
using TechC.InGame.Core;
using TechC.InGame.Log;
using TechC.InGame.Enemy;
using System;

namespace TechC.InGame.Player
{
    public class PlayerMover : MonoBehaviour
    {
        [SerializeField] private Vector2Int _startGridPos;
        private Vector2Int _currentGridPos;
        private float _placementY;

        public static event Action<Map.TileData> OnTileReached;

        private void Start()
        {
            var mapManager = InGameManager.I?.MapManager;
            if (mapManager == null)
            {
                CusLog.Warning("[PlayerMover] MapManager が取得できませんでした。");
                return;
            }

            if (!mapManager.IsValidPosition(_startGridPos))
            {
                CusLog.Warning($"[PlayerMover] 開始グリッド座標 {_startGridPos} が無効です。");
                return;
            }

            _currentGridPos = _startGridPos;
            
            if (PlayerController.I != null) PlayerController.I.CurrentGridPos = _currentGridPos;

            var tile = mapManager.GetTile(_currentGridPos);
            CachePlacementY(tile);

            var tilePos = tile.TileObject.transform.position;
            transform.position = new Vector3(tilePos.x, _placementY, tilePos.z);
        }

        private void CachePlacementY(Map.TileData tile)
        {
            var tileRenderer = tile.TileObject.GetComponent<Renderer>();
            if (tileRenderer == null) return;
            float tileTopY = tileRenderer.bounds.max.y;

            var playerRenderer = GetComponent<Renderer>();
            if (playerRenderer == null) return;
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
            if (mapManager == null) return;

            var nextPos = _currentGridPos + direction;
            
            if (!mapManager.IsValidPosition(nextPos)) return;

            EnemyDataOnTile enemy = mapManager.GetEnemyAt(nextPos);
            if (enemy != null)
            {
                CusLog.Log($"[Encounter] 敵と遭遇！ 戦闘開始 (座標: {nextPos})");
                return; 
            }

            _currentGridPos = nextPos;
            
            if (PlayerController.I != null) PlayerController.I.CurrentGridPos = _currentGridPos;

            var tile = mapManager.GetTile(_currentGridPos);
            var tilePos = tile.TileObject.transform.position;
            transform.position = new Vector3(tilePos.x, _placementY, tilePos.z);
            
            InGameManager.I.ScoreManager.AddMoveSuccessScore();
            OnTileReached?.Invoke(tile);
        }
    }
}