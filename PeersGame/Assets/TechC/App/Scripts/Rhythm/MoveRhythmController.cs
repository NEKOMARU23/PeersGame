using UnityEngine;
using TechC.Audio;

namespace TechC.Rhythm
{
    /// <summary>
    /// 移動フェーズのリズム制御（クリック音＋移動BGM）
    /// </summary>
    public class MoveRhythmController : MonoBehaviour
    {
        [Header("クリック音")]
        [SerializeField] private AudioSource _clickSource;

        [Header("移動BPM設定")]
        [SerializeField] private double _moveBpm = 112.0;

        private BeatTimer _beatTimer;
        private bool _isRunning = false;

        // 最後に予約した拍番号（重複予約防止）
        private double _lastScheduledBeat = double.NegativeInfinity;

        /// <summary>
        /// 移動フェーズのリズムを開始する
        /// </summary>
        public void StartMoveRhythm(AudioData moveBgmData = null)
        {
            _beatTimer = new BeatTimer();
            _lastScheduledBeat = double.NegativeInfinity; // リセット

            if (moveBgmData != null)
            {
                double startTime = AudioManager.Instance.PlayBgmScheduled(moveBgmData);
                _beatTimer.StartAtDspTime(startTime, _moveBpm);
            }
            else
            {
                _beatTimer.Start(_moveBpm, 0.1);
            }

            _isRunning = true;
        }

        private void Update()
        {
            if (!_isRunning || _beatTimer == null || !_beatTimer.IsRunning)
            {
                return;
            }

            // 現在の拍（小数）と次の整数拍番号を取得
            double currentBeat = _beatTimer.GetCurrentBeat();
            double nextBeatNumber = System.Math.Ceiling(currentBeat + 1e-9); // 小さなイプシロンで丸め誤差を防ぐ

            // 次の拍の dspTime を取得
            double nextBeatTime = _beatTimer.GetDspTimeForBeat(nextBeatNumber);
            double dsp = AudioSettings.dspTime;

            // マージン内に入ったら、かつまだ予約していない拍だけ予約する
            const double scheduleMargin = 0.05; // 50ms 前に予約
            if (dsp + scheduleMargin >= nextBeatTime && nextBeatNumber > _lastScheduledBeat)
            {
                _clickSource.PlayScheduled(nextBeatTime);
                _lastScheduledBeat = nextBeatNumber;
            }
        }

        public void Pause()
        {
            _beatTimer?.Pause();
            _isRunning = false;
        }

        public void Resume()
        {
            _beatTimer?.Resume();
            _isRunning = true;
            // Resume 後は次の拍を再予約できるように lastScheduled をリセットしておく
            _lastScheduledBeat = double.NegativeInfinity;
        }

        public void Stop()
        {
            _beatTimer?.Stop();
            _isRunning = false;
            _lastScheduledBeat = double.NegativeInfinity;
        }
    }
}