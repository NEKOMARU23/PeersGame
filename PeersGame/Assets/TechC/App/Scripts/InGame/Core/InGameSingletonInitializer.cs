using TechC.Scene.Manager;
using UnityEngine;

namespace TechC.InGame.Core
{
    /// <summary>
    /// InGameシーンのSingleton初期化順序を管理
    /// </summary>
    [DefaultExecutionOrder(-9999)]
    public class InGameSingletonInitializer : MonoBehaviour
    {
        [Header("InGameシーンで必要なSingleton")]
        [SerializeField] private GameManager   _gameManager; // 別のシーンからでも再生できるように追加をしているが将来的には消す予定
        [SerializeField] private InGameManager _inGameManager;

        private void Awake() => InitializeManagers();

        private void InitializeManagers()
        {
            _gameManager.InitializeSingleton();
            _inGameManager.InitializeSingleton();
        }
    }
}
