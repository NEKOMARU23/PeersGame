using UnityEngine;
using TechC.Audio;

namespace TechC.Rhythm
{
    /// <summary>
    /// 移動フェーズのリズム制御（クリック音＋移動BGM）
    /// </summary>
    using UnityEngine;

public class MoveRhythmController : MonoBehaviour
{
    [SerializeField] private AudioSource _clickSource;
    [SerializeField] private AudioSource _bgmSource;
    [SerializeField] private bool _playOnStart = true;
    [SerializeField] private bool _playBgm = true;
    [SerializeField, Min(1f)] private float _bpm = 120f;
    [SerializeField, Min(0f)] private float _autoStartDelaySec = 0.1f;
    [SerializeField] private float _scheduleMargin = 0.05f;

    private BeatTimer _beatTimer = new BeatTimer();
    private float _lastScheduledBeat = -999f;
    private bool _isRunning = false;
    private float _outputLatencySec;
    private float _bgmStartDspTime;

    public bool IsRunning => _isRunning;

    private void Awake()
    {
        AudioSettings.GetDSPBufferSize(out int bufferSize, out int numBuffers);
        _outputLatencySec = (float)(bufferSize * numBuffers) / AudioSettings.outputSampleRate;

        if (_clickSource != null)
        {
            // クリック音は PlayScheduled のみで鳴らし、開始時の余計な先行再生を防ぐ。
            _clickSource.playOnAwake = false;
            _clickSource.loop = false;
            _clickSource.Stop();
        }

        if (_bgmSource != null)
        {
            _bgmSource.playOnAwake = false;
            _bgmSource.loop = true;
            _bgmSource.Stop();
        }
    }

    private void Start()
    {
        if (!_playOnStart || _isRunning) return;

        float startDspTime = (float)AudioSettings.dspTime + _autoStartDelaySec;
        StartMoveRhythm(startDspTime, _bpm);
    }

    public void StartMoveRhythm(float bgmStartDspTime, float bpm)
    {
        _bgmStartDspTime = bgmStartDspTime;
        _beatTimer.StartAtDspTime(bgmStartDspTime, bpm);
        _lastScheduledBeat = -999f;
        _isRunning = true;

        if (_playBgm && _bgmSource != null)
            _bgmSource.PlayScheduled(bgmStartDspTime);
    }

    public void Pause()
    {
        if (!_isRunning) return;
        _beatTimer.Pause();
        _bgmSource?.Pause();
        _isRunning = false;
    }

    public void Resume()
    {
        if (_isRunning) return;
        _beatTimer.Resume();
        _bgmSource?.UnPause();
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
        _beatTimer.Stop();
        _bgmSource?.Stop();
        _lastScheduledBeat = -999f;
    }

    /// <summary>
    /// BGMの再生ON/OFFを切り替える。実行中でも即反映される。
    /// </summary>
    public void SetBgmEnabled(bool enabled)
    {
        _playBgm = enabled;
        if (_bgmSource == null) return;

        if (!enabled)
        {
            _bgmSource.Stop();
            return;
        }

        if (!_isRunning) return;

        // 現在の再生位置を計算して頭出しせずに同期再生
        float elapsed = (float)AudioSettings.dspTime - _bgmStartDspTime;
        if (elapsed >= 0f && _bgmSource.clip != null)
            _bgmSource.time = elapsed % _bgmSource.clip.length;
        _bgmSource.Play();
    }

    /// <summary>
    /// 入力が拍の判定ウィンドウ内かどうかを返す（±windowSec）。
    /// </summary>
    public bool IsInputWithinBeatWindow(float windowSec)
    {
        if (!_isRunning) return false;

        float currentBeat = _beatTimer.GetCurrentBeat();
        float nearestBeat = Mathf.Round(currentBeat);
        float nearestBeatTime = _beatTimer.GetDspTimeForBeat(nearestBeat) + _outputLatencySec;
        float now = (float)AudioSettings.dspTime;
        float delta = now - nearestBeatTime;

        return delta >= -windowSec && delta <= windowSec;
    }

    private void Update()
    {
        if (!_isRunning) return;
        if (_clickSource == null) return;

        float dsp = (float)AudioSettings.dspTime;
        float currentBeat = _beatTimer.GetCurrentBeat();
        float nextBeat = Mathf.Ceil(currentBeat + 1e-6f);
        float nextBeatTime = _beatTimer.GetDspTimeForBeat(nextBeat);

        if (dsp + _scheduleMargin >= nextBeatTime && nextBeat > _lastScheduledBeat)
        {
            _clickSource.PlayScheduled(nextBeatTime); // float のまま渡す
            _lastScheduledBeat = nextBeat;
        }
    }
}
}