using System;
using Unity.VisualScripting;
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
        [SerializeField] private InGameManager _inGameManager;

        private void Awake()
        {
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            _inGameManager.InitializeSingleton();
        }
    }
}
