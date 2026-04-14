using UnityEngine;

namespace TechC.Audio
{

    /// <summary>
    /// 曲の拍を管理するクラス
    /// </summary>
    public class BeatTimer : MonoBehaviour
    {
        [Header("Status (Read Only)")]
        [SerializeField] private bool _isRunning = false;
        [SerializeField] private float _bpm;
        [SerializeField] private float _currentBeatDisplay; // インスペクター確認用

        private float _startDspTime;
        private float _beatOffset;
        private float _pausedBeat = 0f;

        public bool IsRunning => _isRunning;
        public static BeatTimer Instance;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        private void Update()
        {
            // インスペクターで数字が動くのを確認するためだけの処理
            if (_isRunning)
            {
                _currentBeatDisplay = GetCurrentBeat();
            }
        }

        /// <summary>
        /// BPMと開始遅延を指定してビートタイマーを開始する
        /// </summary>
        /// <param name="bpm"></param>
        /// <param name="delaySec"></param>
        public void Begin(float bpm, float delaySec = 0f)
        {
            _bpm = bpm;
            _startDspTime = (float)AudioSettings.dspTime + delaySec;
            _beatOffset = 0f;
            _isRunning = true;
        }

        /// <summary>
        /// DSP時間を指定してビートタイマーを開始する
        /// これを使うと、例えば曲の特定の位置からビートタイマーを開始することができます。
        /// 例えば、曲のイントロが4小節あって、その後にゲームが始まる場合などに便利です。
        /// 使い方例: StartAtDspTime(AudioSettings.dspTime + 8f, 120f); // 8秒後にBPM120で開始
        /// </summary>
        /// <param name="dspTime"></param>
        /// <param name="bpm"></param>
        /// </summary>
        public void StartAtDspTime(float dspTime, float bpm)
        {
            _bpm = bpm;
            _startDspTime = dspTime;
            _beatOffset = 0f;
            _isRunning = true;
        }

        /// <summary>
        /// 現在のビート数を取得する
        /// </summary>
        /// <returns></returns>
        public float GetCurrentBeat()
        {
            if (!_isRunning)
                return _pausedBeat;

            float dsp = (float)AudioSettings.dspTime;
            float sec = dsp - _startDspTime;
            return sec * _bpm / 60f + _beatOffset;
        }


        /// <summary>
        /// 指定したビート数に対応するDSP時間を取得する
        /// </summary>
        /// <param name="beat"></param>
        /// <returns></returns>
        public float GetDspTimeForBeat(float beat)
        {
            return _startDspTime + ((beat - _beatOffset) * 60f / _bpm);
        }

        /// <summary>
        /// ビートタイマーを一時停止する。再開するときはResume()を呼ぶこと。
        /// </summary>
        public void Pause()
        {
            if (!_isRunning) return;
            _pausedBeat = GetCurrentBeat();
            _isRunning = false;
        }

        /// <summary>
        /// ビートタイマーを再開する。Pause()で一時停止した位置から再開します。
        /// 例えば、ゲームの一時停止メニューから戻るときなどに呼び出すことができます。
        /// 注意: BPMや開始時間は変更されません。Pause()したときの状態をそのまま引き継いで再開します。
        /// </summary>
        public void Resume()
        {
            if (_isRunning) return;

            _startDspTime = (float)AudioSettings.dspTime;
            _beatOffset = _pausedBeat;
            _isRunning = true;
        }

        /// <summary>
        ///     ビートタイマーを完全に停止する。再開はできません。必要なら新しくBegin()してください。
        /// </summary>
        public void Stop()
        {
            _isRunning = false;
            _pausedBeat = 0f;
        }
    }
}