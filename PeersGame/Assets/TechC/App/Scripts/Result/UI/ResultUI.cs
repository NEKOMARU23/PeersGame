using UnityEngine;
using TMPro;
using TechC.Scene.Manager;

namespace TechC.Result.UI
{
    /// <summary>
    /// リザルト画面のUI描画クラス
    /// </summary>
    public class ResultUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _scoreText;

        private void Start()
        {
            _scoreText.text = GameManager.I.ScoreManager.Score.ToString();
        }
    }
}
