using UnityEngine;
using UnityEngine.UI;

namespace TechC.InGame.Player
{
    //public class PlayerController : Singleton<PlayerController>  
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController I { get; private set; }

        [Header("HPの設定")]
        [SerializeField] private int _maxHp = 10; // 最大の体力の値
        [SerializeField] private Image[] _hpImages; // HPの画像
        private int _currentHp; // 現在の体力の値

        private void Awake()
        {
            I = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentHp = _maxHp;
            if (_hpImages.Length != _maxHp)
            {
                Debug.LogError("HP画像の数がmaxHpと一致しません。UIが正しく表示されない可能性があります。");
            }
        }

        public void TakeDamage(int damage) // ダメージを受けたときの処理
        {
            _currentHp -= damage; // ダメージ分HPを減らす
            UpdateHpUI(); // HP画像の更新をする
            if (_currentHp <= 0)
            {
                // HPが0になったときの処理（例: ゲームオーバー）
                Debug.Log("プレイヤーが死亡しました。");
            }
        }

        public void TakeHeal(int heal) // 回復処理
        {
            _currentHp += heal; // 回復分HPを増やす
            if (_currentHp > _maxHp)
            {
                _currentHp = _maxHp; // 最大HPを超えないように
            }
            UpdateHpUI(); // HP画像の更新をする
            Debug.Log($"回復した: +{heal} HP");
        }

        private void UpdateHpUI() // HP画像が減る表示系の関数
        {
            for (int i = 0; i < _hpImages.Length; i++)
            {
                _hpImages[i].enabled = i < _currentHp;
            }
        }
    }
}
