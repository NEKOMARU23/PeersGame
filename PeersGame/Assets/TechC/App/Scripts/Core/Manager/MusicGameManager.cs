using UnityEngine;
using TechC.Audio;
using TechC.Scene.Manager;
using TechC.Core.Manager; // Singleton基底クラスのネームスペース
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
        /// リズムゲームを最初から再生する
        /// </summary>
        public void PlayMusic()
        {
            if (_audioSource != null) _audioSource.Stop();

            if (NoteSpawner.IsValid())
            {
                NoteSpawner.I.ResetSpawner();
            }

            if (BeatTimer.Instance != null)
            {
                BeatTimer.Instance.Begin(_songBpm, _startDelay);
                _audioSource?.PlayDelayed(_startDelay);
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