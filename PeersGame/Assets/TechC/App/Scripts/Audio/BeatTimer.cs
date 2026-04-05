using UnityEngine;

namespace TechC.Audio
{
    public class BeatTimer
    {
        // 公開プロパティ
        public double StartDspTime { get; private set; } = 0.0;
        public double Bpm => _bpm;
        public bool IsRunning => _isRunning;

        // private フィールド
        private double _bpm = 120.0;
        private bool _isRunning = false;
        private double _beatOffset = 0.0; // StartDspTime に対応する beat のオフセット（通常 0）

        // Pause/Resume 用に一時保存する現在の拍
        private double _pausedBeat = 0.0;
        private bool _isPaused = false;

        public double Start(double bpm, double startDelaySec = 0.1)
        {
            _bpm = bpm;
            StartDspTime = AudioSettings.dspTime + startDelaySec;
            _beatOffset = 0.0;
            _isRunning = true;
            _isPaused = false;
            return StartDspTime;
        }

        public void StartAtDspTime(double startDspTime, double bpm)
        {
            _bpm = bpm;
            StartDspTime = startDspTime;
            _beatOffset = 0.0;
            _isRunning = true;
            _isPaused = false;
        }

        public void Stop()
        {
            _isRunning = false;
            _isPaused = false;
        }

        /// <summary>
        /// 一時停止。現在の拍を保存して停止する。
        /// </summary>
        public void Pause()
        {
            if (!_isRunning) return;
            // 現在の拍を保存して停止
            _pausedBeat = GetBeatAtDspTime(AudioSettings.dspTime);
            _isRunning = false;
            _isPaused = true;
        }

        /// <summary>
        /// 再開。保存した拍を基準に StartDspTime とオフセットを補正して拍の連続性を保つ。
        /// </summary>
        public void Resume()
        {
            if (!_isPaused)
            {
                // 既に動作中なら何もしない
                if (_isRunning) return;
                // 停止状態からの単純再開なら現在時刻を基準にする
                StartDspTime = AudioSettings.dspTime;
                _beatOffset = 0.0;
                _isRunning = true;
                _isPaused = false;
                return;
            }

            // 再開時に現在の dspTime を新しい StartDspTime とし、
            // 保存しておいた pausedBeat をオフセットとして設定することで拍を継続する
            double currentDsp = AudioSettings.dspTime;
            _beatOffset = _pausedBeat;
            StartDspTime = currentDsp;
            _isRunning = true;
            _isPaused = false;
        }

        public double GetCurrentBeat()
        {
            if (!_isRunning) return _isPaused ? _pausedBeat : 0.0;
            double dsp = AudioSettings.dspTime;
            return GetBeatAtDspTime(dsp);
        }

        public double GetBeatAtDspTime(double dspTime)
        {
            if (!_isRunning && !_isPaused) return 0.0;
            double secondsSinceStart = dspTime - StartDspTime;
            double beatsSinceStart = secondsSinceStart * _bpm / 60.0;
            return _beatOffset + beatsSinceStart;
        }

        public double GetDspTimeForBeat(double beat)
        {
            double beatsFromStart = beat - _beatOffset;
            double secondsFromStart = beatsFromStart * 60.0 / _bpm;
            return StartDspTime + secondsFromStart;
        }

        public void ChangeBpm(double newBpm, double effectiveDspTime)
        {
            if (!_isRunning)
            {
                _bpm = newBpm;
                return;
            }

            double currentBeat = GetBeatAtDspTime(effectiveDspTime);
            _beatOffset = currentBeat;
            StartDspTime = effectiveDspTime;
            _bpm = newBpm;
        }

        public double GetNextBeatDspTime()
        {
            if (!_isRunning && !_isPaused) return StartDspTime;
            double currentBeat = GetCurrentBeat();
            double nextBeat = System.Math.Floor(currentBeat) + 1.0;
            return GetDspTimeForBeat(nextBeat);
        }
    }
}