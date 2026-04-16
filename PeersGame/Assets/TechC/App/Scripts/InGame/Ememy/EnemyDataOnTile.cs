using UnityEngine;
using TechC.InGame.Log;

namespace TechC.InGame.Enemy
{
    public class EnemyDataOnTile : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }
        private EnemyController _controller;

        public void Setup(Vector2Int pos)
        {
            GridPosition = pos;
            _controller = GetComponent<EnemyController>();
            if (_controller != null)
            {
                _controller.Initialize();
            }
        }

        public void OnPlayerEnter()
        {
            CusLog.Log($"[TileLog] {gameObject.name}: プレイヤーがタイルに進入しました。(座標: {GridPosition})");
            
            if (_controller != null)
            {
                _controller.ShowHealthUI();
            }
            else
            {
                Debug.LogError($"[TileLog] {gameObject.name}: EnemyControllerが見つかりません！");
            }
        }

        public void RequestDamage(int damage)
        {
            if (_controller != null) _controller.TakeDamage(damage);
        }

        public bool IsAlive()
        {
            return _controller != null && _controller.GetCurrentHp() > 0;
        }
    }
}