using UnityEngine;
using TMPro;
using TechC.InGame.Core;
using TechC.Scene.Manager;

namespace TechC.InGame.UI
{
    /// <summary>
    /// InGameシーンでスコアを描画するクラス
    /// </summary>
    public class ScoreRenderer : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void OnEnable()
        {
            if (!InGameManager.IsValid()) return;
            InGameManager.I.ScoreManager.OnScoreChanged += UpdateDisplay;
        }

        private void OnDisable()
        {
            if (!InGameManager.IsValid()) return;
            InGameManager.I.ScoreManager.OnScoreChanged -= UpdateDisplay;
        }

        private void Start()
        {
            UpdateDisplay(InGameManager.I.ScoreManager.Score);
        }

        private void UpdateDisplay(int score)
        {
            _scoreText.text = score.ToString();

            // 将来的には音楽が終了したらこの処理を呼ぶ
            // GameManager.I.SetFinalScore(score);
            // SceneController.I.ChangeResultScene();
        }
    }
}
