using UnityEngine;
using UnityEngine.UI;

namespace TechC.InGame.Player
{
    public class PlayerStatus : MonoBehaviour
    {
        [Header("HPの設定")]
        [SerializeField] private int maxHp = 10; // 最大の体力の値
        [SerializeField] private Image[] hpImages; // HPの画像
        private int currentHp; // 現在の体力の値

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            currentHp = maxHp;
            if (hpImages.Length != maxHp)
            {
                Debug.LogError("HP画像の数がmaxHpと一致しません。UIが正しく表示されない可能性があります。");
            }
        }

        // Update is called once per frame
        void Update()
        {
            // 必要に応じて更新処理を追加
        }

        public void TakeDamage(int damage) // ダメージを受けたときの処理
        {
            currentHp -= damage; // ダメージ分HPを減らす
            UpdateHpUI(); // HP画像の更新をする
            if (currentHp <= 0)
            {
                // HPが0になったときの処理（例: ゲームオーバー）
                Debug.Log("プレイヤーが死亡しました。");
            }
        }

        public void TakeHeal(int heal) // ダメージを受けたときの処理
        {
            currentHp += heal; // ダメージ分HPを減らす
            UpdateHpUI(); // HP画像の更新をする
            Debug.Log("回復した");
        }

        private void UpdateHpUI() // HP画像が減る表示系の関数
        {
            for (int i = 0; i < hpImages.Length; i++)
            {
                hpImages[i].enabled = i < currentHp;
            }
        }
    }
}
