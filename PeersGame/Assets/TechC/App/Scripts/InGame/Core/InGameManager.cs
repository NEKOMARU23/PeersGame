using UnityEngine;
using TechC.Core.Manager;
using TechC.InGame.Map;
using TechC.InGame.Log;
using TechC.InGame.Score;
using TechC.Core.Events;

namespace TechC.InGame.Core
{
    public enum GamePhaseType
    {
        Playing,
        Paused
    }

    /// <summary>
    /// InGameシーンの管理クラス
    /// </summary>
    public class InGameManager : Singleton<InGameManager>
    {
        protected override bool UseDontDestroyOnLoad => false;

        [SerializeField] private MapGenerator _mapGenerator;
        [SerializeField] private ScoreDataSO _scoreData;

        public GamePhaseType CurrentPhase { get; private set; } = GamePhaseType.Playing;
        public MapManager MapManager { get; private set; }
        public ScoreManager ScoreManager { get; private set; }

        protected override void OnInitialize()
        {
            ScoreManager = new ScoreManager(_scoreData);
            GenerateMap();
            
            // 開始時は通常速度
            Time.timeScale = 1f; 
        }

        public void TogglePause()
        {
            if (CurrentPhase == GamePhaseType.Paused) ResumeGame();
            else PauseGame();
        }

        private void PauseGame()
        {
            CurrentPhase = GamePhaseType.Paused;
            Time.timeScale = 0f; // 時間を止める
            
            CusLog.Log("[InGameManager] Game Paused");
            
            // 型安全なイベント発行
            EventBus.Publish(new PhaseChangedEvent(GamePhaseType.Paused));
        }

        private void ResumeGame()
        {
            CurrentPhase = GamePhaseType.Playing;
            Time.timeScale = 1f; // 時間を再開
            
            CusLog.Log("[InGameManager] Game Resumed");
            
            // 型安全なイベント発行
            EventBus.Publish(new PhaseChangedEvent(GamePhaseType.Playing));
        }

        private void GenerateMap()
        {
            if (_mapGenerator == null)
            {
                CusLog.Error("[InGameManager] MapGenerator がアサインされていません。");
                return;
            }

            MapManager = _mapGenerator.Generate();
        }
    }

    /// <summary>
    /// フェーズ変更イベント。IGameEventを継承してEventBusに対応
    /// </summary>
    public class PhaseChangedEvent : IGameEvent
    {
        public GamePhaseType NewPhase { get; private set; }
        public PhaseChangedEvent(GamePhaseType phase) => NewPhase = phase;
    }
}