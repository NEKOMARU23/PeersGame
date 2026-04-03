using UnityEngine;
using TechC.Scene.Manager;
using TechC.Audio;

namespace TechC.Core.Scene
{
    /// <summary>
    /// ManagerSceneでのSingleton初期化順序を管理
    /// </summary>
    [DefaultExecutionOrder(-5000)]
    public class ManagerSingletonInitializer : MonoBehaviour
    {
        private void Awake()
        {
            InitializeManagers();
        }

        private void InitializeManagers()
        {
            SceneController.Init();
            AudioManager.Init();
        }
    }
}