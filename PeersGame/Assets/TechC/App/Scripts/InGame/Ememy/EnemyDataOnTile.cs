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

        public void RequestDamage(int damage)
        {
            if (_controller != null)
            {
                _controller.TakeDamage(damage);
            }
        }

        public bool IsAlive()
        {
            if (_controller != null)
            {
                return _controller.GetCurrentHp() > 0;
            }
            return false;
        }

        public void OnPlayerEnter()
        {
            CusLog.Log($"敵と遭遇！座標: {GridPosition}");
        }
    }
}