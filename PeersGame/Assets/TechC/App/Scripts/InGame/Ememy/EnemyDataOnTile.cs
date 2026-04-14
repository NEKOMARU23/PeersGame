using TechC.InGame.Log;
using UnityEngine;

namespace TechC.InGame.Enemy
{
    public class EnemyDataOnTile : MonoBehaviour
    {
        public Vector2Int GridPosition { get; private set; }
        
        [SerializeField] private int _maxHp = 3;
        private int _currentHp;

        public void Setup(Vector2Int pos)
        {
            GridPosition = pos;
            _currentHp = _maxHp;
        }


        public void OnPlayerEnter()
        {
            CusLog.Log($"敵と遭遇！座標: {GridPosition}");
        }
    }
}