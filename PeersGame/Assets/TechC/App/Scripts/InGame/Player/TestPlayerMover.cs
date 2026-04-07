using UnityEngine;
using UnityEngine.InputSystem;
using TechC.InGame.Core;
using TechC.InGame.Log;

namespace TechC.InGame.Player
{
    /// <summary>
    /// WASDによるグリッド移動のテスト用クラス
    /// InGameManager.I経由でMapManagerを参照し、有効なマスにのみ移動する
    /// </summary>
    public class TestPlayerMover : MonoBehaviour
    {
        /// <summary>現在のグリッド座標</summary>
        private Vector2Int _currentGridPos;

        private void Start()
        {
            _currentGridPos = new Vector2Int(
                Mathf.RoundToInt(transform.position.x),
                Mathf.RoundToInt(transform.position.z)
            );
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
            transform.position = new Vector3(_currentGridPos.x, transform.position.y, _currentGridPos.y);
        }
    }
}
