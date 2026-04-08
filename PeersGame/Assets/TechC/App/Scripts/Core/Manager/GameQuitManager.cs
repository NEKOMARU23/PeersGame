using UnityEngine;

namespace TechC.Scene.Manager
{
    /// <summary>
    /// ゲーム終了を管理するクラス
    /// </summary>
    public class GameQuitManager : MonoBehaviour
    {
        /// <summary>
        /// ゲームを終了する。
        /// UnityエディタのButtonコンポーネントから呼び出すためにpublicで定義します。
        /// </summary>
        public void OnClickQuit()
        {
            // Unityエディタ上での動作確認用
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 実際にビルドされたアプリを終了させる
            Application.Quit();
#endif
        }
    }
}