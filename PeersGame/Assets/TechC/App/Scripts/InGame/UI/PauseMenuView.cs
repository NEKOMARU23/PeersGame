using UnityEngine;
using UnityEngine.UI;
using TechC.Core.Events;
using TechC.InGame.Core;
using TechC.InGame.Log;
using UnityEngine.SceneManagement;

namespace TechC.InGame.UI
{
    /// <summary>
    /// 一時停止メニューの表示とユーザー入力を管理するUIクラス
    /// </summary>
    public class PauseMenuView : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private GameObject _rootPanel;
        [SerializeField] private Button _resumeButton;
        [SerializeField] private Button _titleButton;
        [SerializeField] private Button _quitButton;
        [SerializeField] private Button _openMenuButton;

        private void Start()
        {
            if (_rootPanel != null)
            {
                // 初期状態をInGameManagerと同期
                bool isPaused = (InGameManager.I != null && InGameManager.I.CurrentPhase == GamePhaseType.Paused);
                _rootPanel.SetActive(isPaused);
            }
        }

        private void OnEnable()
        {
            // 型安全なEventBusへの登録
            EventBus.Subscribe<PhaseChangedEvent>(OnPhaseChanged);

            if (_resumeButton != null) _resumeButton.onClick.AddListener(OnClickResume);
            if (_titleButton != null) _titleButton.onClick.AddListener(OnClickTitle);
            if (_quitButton != null) _quitButton.onClick.AddListener(OnClickQuit);
            if (_openMenuButton != null) _openMenuButton.onClick.AddListener(OnClickOpenMenu);
        }

        private void OnDisable()
        {
            // 解除を忘れない
            EventBus.Unsubscribe<PhaseChangedEvent>(OnPhaseChanged);

            if (_resumeButton != null) _resumeButton.onClick.RemoveListener(OnClickResume);
            if (_titleButton != null) _titleButton.onClick.RemoveListener(OnClickTitle);
            if (_quitButton != null) _quitButton.onClick.RemoveListener(OnClickQuit);
            if (_openMenuButton != null) _openMenuButton.onClick.RemoveListener(OnClickOpenMenu);
        }

        private void OnPhaseChanged(PhaseChangedEvent e)
        {
            if (_rootPanel != null)
            {
                bool shouldShow = (e.NewPhase == GamePhaseType.Paused);
                _rootPanel.SetActive(shouldShow);
                
                if(shouldShow) CusLog.Log("[UI] 一時停止メニューを表示しました。");
            }
        }

        private void OnClickOpenMenu()
        {
            if (_rootPanel != null && _rootPanel.activeSelf) return;

            InGameManager.I.TogglePause();
        }

        private void OnClickResume()
        {
            if (_rootPanel != null && !_rootPanel.activeSelf) return;

            InGameManager.I.TogglePause();
        }

        private void OnClickTitle()
        {
            CusLog.Log("[UI] タイトル画面へ戻ります。");

            // ポーズ解除（Time.timeScaleを1に戻す）してから遷移
            if (InGameManager.I != null && InGameManager.I.CurrentPhase == GamePhaseType.Paused)
            {
                InGameManager.I.TogglePause();
            }

            SceneManager.LoadScene("TitleScene");
        }

        private void OnClickQuit()
        {
            CusLog.Log("[UI] アプリケーションを終了します。");

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }
}