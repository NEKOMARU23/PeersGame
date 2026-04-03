using TechC.Scene.Manager;
using UnityEngine;

namespace TechC.UI
{
    /// <summary>
    /// ButtonのOnClickからシーンを変更するためのラッパークラス
    /// </summary>
    public class SceneChangeButtonHandler : MonoBehaviour
    {
        [SerializeField]
        private SceneName _targetScene = SceneName.Title;

        /// <summary>
        /// ButtonのOnClickから呼び出されるメソッド
        /// 指定されたシーンに切り替える
        /// </summary>
        public void ChangeScene()
        {
            if (SceneController.I == null) return;
            
            // _targetSceneに応じて適切なメソッドを呼び出す
            switch (_targetScene)
            {
                case SceneName.Title:
                    SceneController.I.ChangeToTitleScene();
                    break;
                case SceneName.InGame:
                    SceneController.I.ChangeToInGameScene();
                    break;
                default:
                    SceneController.I.LoadScene(_targetScene);
                    break;
            }
        }
    }
}