using UnityEngine;
using UnityEngine.UI;
using TMPro;
using TechC.InGame.Log;
using TechC.InGame.Enemy;
using TechC.InGame.Player;
using TechC.InGame.Notes;
using TechC.Scene.Manager;
using UnityEngine.InputSystem;

namespace TechC.InGame.Core
{
    /// <summary>
    /// 戦闘シーンの進行とクリア判定を管理するクラス
    /// </summary>
    public class BattleManager : MonoBehaviour
    {
        public static BattleManager I { get; private set; }

        [Header("View References")]
        [SerializeField] private GameObject _fieldView;  
        [SerializeField] private GameObject _battleView;
        
        [Header("Battle Elements")]
        [SerializeField] private Image _battleEnemyImage;
        [SerializeField] private GameObject _noteSystemRoot;
        
        [Header("UI Elements")]
        [SerializeField] private TextMeshProUGUI _guidanceText; 

        private EnemyDataOnTile _currentEnemy;
        private bool _isBattleActive = false; 

        private void Awake()
        {
            I = this;
            _fieldView.SetActive(true);
            _battleView.SetActive(false);
            if (_noteSystemRoot != null) _noteSystemRoot.SetActive(false);
        }

        private void Update()
        {
            if (!_isBattleActive) return;

            // 音楽が再生されていない ＝ 入力待ち状態
            if (MusicGameManager.I != null && !MusicGameManager.I.IsPlaying)
            {
                if (GetStartInput())
                {
                    SetGuidance(""); 
                    MusicGameManager.I.PlayMusic();
                }
            }
        }

        private void SetGuidance(string message)
        {
            if (_guidanceText != null)
            {
                _guidanceText.text = message;
            }
        }

        public void OnPhraseResolved(bool attackSuccess, bool defenseSuccess)
        {
            if (MusicGameManager.I != null) MusicGameManager.I.StopMusic();

            if (attackSuccess && _currentEnemy != null)
            {
                _currentEnemy.RequestDamage(1); 
            }

            if (!defenseSuccess && PlayerController.I != null)
            {
                PlayerController.I.TakeDamage(1);
            }

            // 敵が生きているかどうかの判定
            if (_currentEnemy != null && _currentEnemy.IsAlive()) 
            {
                SetGuidance("PRESS SPACE TO NEXT TURN");
                
                if (NoteSpawner.I != null)
                {
                    NoteSpawner.I.PrepareNextLoop();
                }
            }
            else
            {
                // 敵が死亡した、または存在しない場合は戦闘終了
                EndBattle(true);
            }
        }

        public void StartBattle(EnemyDataOnTile enemy)
        {
            if (MusicGameManager.I != null) MusicGameManager.I.StopMusic();

            _currentEnemy = enemy;
            _isBattleActive = true;
            
            _fieldView.SetActive(false); 
            _battleView.SetActive(true);
            if (_noteSystemRoot != null) _noteSystemRoot.SetActive(true);

            SetGuidance("PRESS SPACE TO START");

            if (_battleEnemyImage != null && enemy != null)
            {
                var enemyRenderer = enemy.GetComponentInChildren<SpriteRenderer>();
                if (enemyRenderer != null) _battleEnemyImage.sprite = enemyRenderer.sprite;
            }

            if (NoteSpawner.I != null) NoteSpawner.I.ResetSpawner();
            if (PlayerMover.I != null) PlayerMover.I.enabled = false;
        }

        public void EndBattle(bool isVictory)
        {
            _isBattleActive = false;
            SetGuidance(""); 
            
            if (MusicGameManager.I != null) MusicGameManager.I.StopMusic();
            if (NoteSpawner.I != null) NoteSpawner.I.ResetSpawner();

            if (isVictory && _currentEnemy != null)
            {
                // 1. マップデータから削除（MapManager内部のカウントが減る）
                var mapManager = InGame.Core.InGameManager.I.MapManager;
                
                // 削除前のカウントをログ出力（デバッグ用）
                int beforeCount = mapManager.GetEnemyCount();
                
                mapManager.RemoveEnemyAt(_currentEnemy.GridPosition);

                // 2. クリア判定
                int remaining = mapManager.GetEnemyCount();
                
                CusLog.Log($"[BattleEnd] 敵を撃破。削除前:{beforeCount} -> 残り:{remaining}");

                if (remaining <= 0)
                {
                    CusLog.Log("★全滅を確認しました。タイトルシーンへ遷移します。");
                    if (SceneController.I != null)
                    {
                        SceneController.I.ChangeToTitleScene();
                    }
                    else
                    {
                        CusLog.Error("SceneControllerが見つかりません。");
                    }
                    return; // シーン遷移するのでここで終了
                }
            }

            // まだ敵が残っている、または敗北した場合はフィールドへ戻る
            CusLog.Log("フィールドへ戻ります。");
            _fieldView.SetActive(true);
            _battleView.SetActive(false);
            if (_noteSystemRoot != null) _noteSystemRoot.SetActive(false);
            if (PlayerMover.I != null) PlayerMover.I.enabled = true;

            _currentEnemy = null;
        }

        private bool GetStartInput()
        {
            var kb = Keyboard.current;
            return kb != null && kb.spaceKey.wasPressedThisFrame;
        }
    }
}