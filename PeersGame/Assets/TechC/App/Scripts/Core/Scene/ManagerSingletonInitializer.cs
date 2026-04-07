using UnityEngine;
using TechC.Scene.Manager;
using TechC.Audio;

namespace TechC.Core.Scene
{
    /// <summary>
    /// ManagerSceneでのSingleton初期化順序を管理
    /// </summary>
    [DefaultExecutionOrder(-4999)]
    public class ManagerSingletonInitializer : MonoBehaviour
    {
        [Header("ManagerSceneで必要なSingleton")]
        [SerializeField] private SceneController _sceneController;
        [SerializeField] private AudioManager    _audioManager;
        [SerializeField] private GameManager     _gameManager;

        private void Awake()
        {
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            _sceneController.InitializeSingleton();
            _audioManager.InitializeSingleton();
            _gameManager.InitializeSingleton();
        }
    }
}