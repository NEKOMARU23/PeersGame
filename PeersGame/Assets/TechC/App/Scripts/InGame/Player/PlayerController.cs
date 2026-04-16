using UnityEngine;
using UnityEngine.UI;
using TechC.InGame.Log;

namespace TechC.InGame.Player
{
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController I { get; private set; }

        [Header("HPの設定")]
        [SerializeField] private int _maxHp = 10;
        [SerializeField] private Image[] _hpImages;
        private int _currentHp;

        public Vector2Int CurrentGridPos { get; set; } = Vector2Int.zero;

        private void Awake()
        {
            I = this;
        }

        void Start()
        {
            _currentHp = _maxHp;
            if (_hpImages.Length != _maxHp)
            {
                CusLog.Error("HP画像の数がmaxHpと一致しません。UIが正しく表示されない可能性があります。");
            }
        }

        public void TakeDamage(int damage)
        {
            _currentHp -= damage;

            CusLog.Log($"プレイヤーがダメージを受けた！ 残りHP: {_currentHp}");
            UpdateHpUI();
            if (_currentHp <= 0)
            {
                CusLog.Log("プレイヤーが死亡しました。");
            }
        }

        public void TakeHeal(int heal)
        {
            _currentHp += heal;
            if (_currentHp > _maxHp)
            {
                _currentHp = _maxHp;
            }
            UpdateHpUI();
            CusLog.Log($"回復した: +{heal} HP");
        }

        private void UpdateHpUI()
        {
            for (int i = 0; i < _hpImages.Length; i++)
            {
                if (_hpImages[i] != null) // nullチェック追加
                {
                    _hpImages[i].enabled = i < _currentHp;
                }
            }
        }
    }
}