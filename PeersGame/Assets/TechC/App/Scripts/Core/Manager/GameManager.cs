using UnityEngine;
using TechC.Core.Manager;

namespace TechC.Scene.Manager
{
    /// <summary>
    /// ゲーム全体を管理するクラス
    /// シーンをまたいで情報を保持する
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        /// <summary>ゲーム終了時に保存する最終スコア</summary>
        public int FinalScore { get; private set; }

        /// <summary>ゲーム終了時に最終スコアを保存する</summary>
        public void SetFinalScore(int score) => FinalScore = score;
    }
}
