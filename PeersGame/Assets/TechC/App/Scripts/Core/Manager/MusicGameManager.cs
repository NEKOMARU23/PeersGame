using UnityEngine;
using TechC.Audio;
using TechC.Scene.Manager;
using TechC.Core.Manager; 
using TechC.InGame.Notes;

namespace TechC.InGame
{
    public class MusicGameManager : Singleton<MusicGameManager>
    {
        [Header("Audio Settings")]
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private float _songBpm = 120f;
        [SerializeField] private float _startDelay = 2f;

        private int _currentScore = 0;

        protected override bool UseDontDestroyOnLoad => false;

        /// <summary>
        /// 修正ポイント：警告(CS0114)の解決と初期化の保証
        /// </summary>
        protected override void Awake()
        {
            // 必ず基底クラスのAwakeを呼んで、シングルトンのInstanceを登録させる
            base.Awake();

            // 1体目遭遇時にタイマーが変な挙動をしないよう確実に止めておく
            if (BeatTimer.Instance != null)
            {
                BeatTimer.Instance.Stop();
            }
        }

        /// <summary>
        /// 現在音楽が再生中かどうか（BeatTimerの稼働状況を基準にする）
        /// </summary>
        public bool IsPlaying => BeatTimer.Instance != null && BeatTimer.Instance.IsRunning;

        /// <summary>
        /// リズムゲーム（音楽とタイマー）を再生する
        /// </summary>
        public void PlayMusic()
        {
            // 二重再生防止のため一度止める
            StopMusic();

            if (BeatTimer.Instance != null)
            {
                // タイマー開始
                BeatTimer.Instance.Begin(_songBpm, _startDelay);
                
                // 音楽再生（遅延開始）
                if (_audioSource != null)
                {
                    _audioSource.PlayDelayed(_startDelay);
                }
            }
        }

        /// <summary>
        /// 音楽とタイマーを停止する
        /// </summary>
        public void StopMusic()
        {
            if (_audioSource != null) _audioSource.Stop();
            
            if (BeatTimer.Instance != null)
            {
                BeatTimer.Instance.Stop();
            }
        }

        public void FinishGame()
        {
            if (GameManager.I != null)
            {
                GameManager.I.SetFinalScore(_currentScore);
            }
        }
    }
}