using UnityEngine;
using TMPro;
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
            GameManager.I.ScoreManager.OnScoreChanged += UpdateDisplay;
        }

        private void OnDisable()
        {
            if (!GameManager.IsValid()) return;
            GameManager.I.ScoreManager.OnScoreChanged -= UpdateDisplay;
        }

        private void Start()
        {
            UpdateDisplay(GameManager.I.ScoreManager.Score);
        }

        private void UpdateDisplay(int score)
        {
            _scoreText.text = score.ToString();
            
            if (score < 30) return;
            SceneController.I.ChangeResultScene();
        }
    }
}
