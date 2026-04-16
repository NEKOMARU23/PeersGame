using UnityEngine;
using UnityEngine.UI;
using TMPro; // TextMeshProを使用する場合
using TechC.InGame.Log;
using TechC.InGame.Enemy;
using TechC.InGame.Player;
using TechC.InGame.Notes;
using UnityEngine.InputSystem;

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
        
        // ★追加：ガイダンス用テキスト
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

            // 音楽が再生されていない＝入力待ち状態
            if (MusicGameManager.I != null && !MusicGameManager.I.IsPlaying)
            {
                if (GetStartInput())
                {
                    SetGuidance(""); // ★リズムゲーム開始時にテキストを消す
                    MusicGameManager.I.PlayMusic();
                }
            }
        }

        /// <summary>
        /// ガイダンステキストを書き換える
        /// </summary>
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

            // 判定処理（省略せずそのまま残してください）
            if (attackSuccess && _currentEnemy != null) _currentEnemy.RequestDamage(1); 
            if (!defenseSuccess && PlayerController.I != null) PlayerController.I.TakeDamage(1);

            if (_currentEnemy != null && _currentEnemy.IsAlive()) 
            {
                // ★フレーズ終了後に再度テキストを表示
                SetGuidance("SPACE KEY TO NEXT TURN");
                
                if (NoteSpawner.I != null) NoteSpawner.I.PrepareNextLoop();
            }
            else
            {
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

            // ★戦闘開始時のメッセージ
            SetGuidance("PRESS SPACE TO START");

            // （画像反映や移動停止の処理はそのまま）
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
            SetGuidance(""); // 終了時は消す
            
            if (MusicGameManager.I != null) MusicGameManager.I.StopMusic();
            if (NoteSpawner.I != null) NoteSpawner.I.ResetSpawner();

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