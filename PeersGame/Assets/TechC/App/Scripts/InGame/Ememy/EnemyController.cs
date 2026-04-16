using UnityEngine;
using TechC.InGame.Log;
using TechC.InGame.UI;

namespace TechC.InGame.Enemy
{
    public class EnemyController : MonoBehaviour
    {
        [SerializeField] private int _maxHp = 3;
        private int _currentHp;

        [SerializeField] private HealthUI _enemyHealthUI;

        public int GetCurrentHp() => _currentHp;

        // データの初期化（Setupから呼ばれる）
        public void Initialize()
        {
            _currentHp = _maxHp;
        }

        // UIの表示・上書き（遭遇時に呼ばれる）
        public void ShowHealthUI()
        {
            if (_enemyHealthUI != null)
            {
                _enemyHealthUI.Setup(_maxHp);
                _enemyHealthUI.UpdateDisplay(_currentHp);
                CusLog.Log($"[UI] {gameObject.name} のHPを表示しました。");
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHp -= damage;
            if (_enemyHealthUI != null) _enemyHealthUI.UpdateDisplay(_currentHp);
            if (_currentHp <= 0) Die();
        }

        private void Die()
        {
            if (_enemyHealthUI != null) _enemyHealthUI.UpdateDisplay(0);
            gameObject.SetActive(false);
        }
    }
}