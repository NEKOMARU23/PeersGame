using UnityEngine;
using UnityEngine.UI;
using TechC.InGame.Log;
using TechC.InGame.Enemy;
using TechC.InGame.Player;
using TechC.InGame.Notes;

namespace TechC.InGame.Core
{
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager I { get; private set; }

        [Header("View References")]
        [SerializeField] private GameObject _fieldView;  
        [SerializeField] private GameObject _battleView;
        
        [Header("Battle Elements")]
        [SerializeField] private Image _battleEnemyImage;
        [SerializeField] private GameObject _noteSystemRoot;

        private EnemyDataOnTile _currentEnemy;

        private void Awake()
        {
            I = this;
            _fieldView.SetActive(true);
            _battleView.SetActive(false);
            
            if (_noteSystemRoot != null) _noteSystemRoot.SetActive(false);
        }

        /// <summary>
        /// NoteSpawner からフレーズ終了時に呼ばれる
        /// </summary>
        public void OnPhraseResolved(bool attackSuccess, bool defenseSuccess)
        {
            if (attackSuccess)
            {
                if (_currentEnemy != null)
                {
                    _currentEnemy.RequestDamage(1); 
                }
            }

            if (!defenseSuccess)
            {
                if (PlayerController.I != null)
                {
                    PlayerController.I.TakeDamage(1);
                }
            }
        }

        public void StartBattle(EnemyDataOnTile enemy)
        {
            _currentEnemy = enemy;
            CusLog.Log($"[BattleManager] 戦闘開始: {enemy.GridPosition}");

            _fieldView.SetActive(false); 
            _battleView.SetActive(true);

            if (_battleEnemyImage != null && enemy != null)
            {
                var enemyRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
                if (enemyRenderer != null)
                {
                    _battleEnemyImage.sprite = enemyRenderer.sprite;
                }
            }

            if (_noteSystemRoot != null) 
            {
                _noteSystemRoot.SetActive(true);

                if (MusicGameManager.I != null)
                {
                    MusicGameManager.I.PlayMusic(); 
                }
            }

            if (PlayerMover.I != null) PlayerMover.I.enabled = false;
        }

        public void EndBattle(bool isVictory)
        {
            CusLog.Log($"[BattleManager] 戦闘終了: 勝利={isVictory}");

            if (NoteSpawner.I != null)
            {
                NoteSpawner.I.ResetSpawner();
            }

            if (isVictory && _currentEnemy != null)
            {
                var mapManager = InGame.Core.InGameManager.I.MapManager;
                mapManager.RemoveEnemyAt(_currentEnemy.GridPosition);
            }

            _fieldView.SetActive(true);
            _battleView.SetActive(false);
            if (_noteSystemRoot != null) _noteSystemRoot.SetActive(false);
            if (PlayerMover.I != null) PlayerMover.I.enabled = true;

            _currentEnemy = null;
        }
    }
}