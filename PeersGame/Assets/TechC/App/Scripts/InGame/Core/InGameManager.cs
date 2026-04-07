using UnityEngine;
using TechC.Core.Manager;
using TechC.InGame.Map;
using TechC.InGame.Log;

namespace TechC.InGame.Core
{
    /// <summary>
    /// InGameシーンの管理クラス
    /// MapManagerを保持し、InGame全体の管理を行う
    /// </summary>
    public class InGameManager : Singleton<InGameManager>
    {
        protected override bool UseDontDestroyOnLoad => false;

        [SerializeField] private MapGenerator _mapGenerator;

        /// <summary>
        /// マップ状態管理クラス
        /// </summary>
        public MapManager MapManager { get; private set; }

        protected override void OnInitialize()
        {
            GameManager.I.ScoreManager.Reset();
            GenerateMap();
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
}
