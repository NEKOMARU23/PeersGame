using UnityEngine;
using TechC.Audio;
using TechC.Scene.Manager; // GameManagerにアクセスするため

namespace TechC.InGame
{
    public class MusicGameManager : MonoBehaviour
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _songBpm = 120f;
        [SerializeField] private float _startDelay = 2f;

        private int _currentScore = 0; // 今回の演奏のスコア

        private void Start()
        {
            // 演奏開始！
            if (BeatTimer.Instance != null)
            {
                BeatTimer.Instance.Begin(_songBpm, _startDelay);
                _audioSource?.PlayDelayed(_startDelay);
            }
        }

        // 演奏終了時に呼ぶ
        public void FinishGame()
        {
            // シーンをまたいでも消えない GameManager にスコアを預ける
            if (GameManager.I != null)
            {
                GameManager.I.SetFinalScore(_currentScore);
            }
            // ここでリザルトシーンへ遷移する処理などを書く
        }
    }
}