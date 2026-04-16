using UnityEngine;
using UnityEngine.UI;
using TechC.InGame.Log;
using System.Collections.Generic;

namespace TechC.InGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController I { get; private set; }

        [Header("HPの設定")]
        [SerializeField] private int _maxHp = 5;
        [SerializeField] private int _currentHp;

        [Header("UIの参照 (HealthUIの代わり)")]
        [SerializeField] private GameObject _heartPrefab; // Assets内のプレハブを入れる
        [SerializeField] private Transform _heartParent;  // HierarchyのParentを入れる

        private List<GameObject> _heartList = new List<GameObject>();

        public Vector2Int CurrentGridPos { get; set; } = Vector2Int.zero;

        private void Awake()
        {
            I = this;
            // シーン内にPlayerが複数いるミスを防ぐ
            CusLog.Log("PlayerController: Awake が実行されました。");
        }

        void Start()
        {
            _currentHp = _maxHp;
            CusLog.Log($"PlayerController: Start開始。初期HP: {_currentHp}");
            
            InitializeHpUI();
        }

        private void InitializeHpUI()
        {
            // --- 徹底チェック ---
            if (_heartPrefab == null)
            {
                CusLog.Error("【重要】Heart Prefab がインスペクターで設定されていません！");
                return;
            }
            if (_heartParent == null)
            {
                CusLog.Error("【重要】Heart Parent (親オブジェクト) が設定されていません！");
                return;
            }

            // 既存クリア
            foreach (var h in _heartList) if(h != null) Destroy(h);
            _heartList.Clear();

            // 生成ループ
            for (int i = 0; i < _maxHp; i++)
            {
                GameObject heart = Instantiate(_heartPrefab, _heartParent);
                
                // 生成されたオブジェクトがどこに飛んでいったか追跡
                if (heart != null)
                {
                    heart.name = $"Heart_{i}";
                    _heartList.Add(heart);
                }
            }

            CusLog.Log($"{_heartList.Count} 個のハートを生成しました。Parent: {_heartParent.name}");
        }

        public void TakeDamage(int damage)
        {
            _currentHp -= damage;
            CusLog.Log($"プレイヤーがダメージを受けた！ 残りHP: {_currentHp}");
            
            UpdateHpUI();

            if (_currentHp <= 0)
            {
                _currentHp = 0;
                CusLog.Log("プレイヤーが死亡しました。");
            }
        }

        public void TakeHeal(int heal)
        {
            _currentHp += heal;
            if (_currentHp > _maxHp) _currentHp = _maxHp;
            
            UpdateHpUI();
            CusLog.Log($"回復した: +{heal} HP");
        }

        private void UpdateHpUI()
        {
            for (int i = 0; i < _heartList.Count; i++)
            {
                if (_heartList[i] != null)
                {
                    // 現在のHPより小さいインデックスのハートを表示
                    _heartList[i].SetActive(i < _currentHp);
                }
            }
        }
    }
}