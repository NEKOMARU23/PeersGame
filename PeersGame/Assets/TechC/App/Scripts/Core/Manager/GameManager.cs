using UnityEngine;
using TechC.Core.Manager;
using TechC.InGame.Score;
namespace TechC.Scene.Manager
{
    /// <summary>
    /// ゲーム全体を管理するクラス
    /// シーンをまたいで情報を保持する
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        protected override bool UseDontDestroyOnLoad => true;

        [SerializeField] private ScoreDataSO _scoreData;

        /// <summary>
        /// スコア管理クラス
        /// </summary>
        public ScoreManager ScoreManager { get; private set; }

        protected override void OnInitialize()
        {
            ScoreManager = new ScoreManager(_scoreData);
        }
    }
}
