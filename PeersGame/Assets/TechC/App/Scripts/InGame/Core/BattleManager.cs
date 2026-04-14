using UnityEngine;
using UnityEngine.UI;
using TechC.InGame.Log;
using TechC.InGame.Enemy;
using TechC.InGame.Player;
using TechC.InGame.Notes; // NoteSpawnerにアクセスするために追加

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
        /// 戦闘開始
        /// </summary>
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
                    // 再生命令（この中で NoteSpawner の index リセットが走る）
                    MusicGameManager.I.PlayMusic(); 
                }
            }

            if (PlayerMover.I != null) PlayerMover.I.enabled = false;
        }

        /// <summary>
        /// 戦闘終了
        /// </summary>
        public void EndBattle(bool isVictory)
        {
            CusLog.Log($"[BattleManager] 戦闘終了: 勝利={isVictory}");

            // ★追加：戦闘が終わったので、今出ているノーツをお片付け（回収）する
            // NoteSpawner側もシンプルなシングルトン(public static NoteSpawner I)であることを想定
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