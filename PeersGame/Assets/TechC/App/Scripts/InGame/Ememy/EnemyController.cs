using UnityEngine;
using TechC.InGame.Log;

namespace TechC.InGame.Enemy
{
    /// <summary>
    /// 敵の戦闘ロジック（HP管理・ダメージ・死亡）を担当するクラス
    /// </summary>
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private int _maxHp = 3;
        private int _currentHp;
        public int GetCurrentHp() => _currentHp;

        public void Initialize()
        {
            _currentHp = _maxHp;
            CusLog.Log($"{gameObject.name} が戦闘準備完了。HP: {_currentHp}");
        }
        

        public void TakeDamage(int damage)
        {
            _currentHp -= damage;
            CusLog.Log($"{gameObject.name} に {damage} ダメージ！ 残りHP: {_currentHp}");

            if (_currentHp <= 0)
            {
                Die();
            }
        }

        private void Die()
        {
            CusLog.Log($"{gameObject.name} を撃破！");
            gameObject.SetActive(false);
        }
    }
}