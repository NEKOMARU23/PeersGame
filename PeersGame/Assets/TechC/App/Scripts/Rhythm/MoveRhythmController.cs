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
    [SerializeField] private float _scheduleMargin = 0.05f;

    private BeatTimer _beatTimer = new BeatTimer();
    private float _lastScheduledBeat = -999f;
    private bool _isRunning = false;

    public void StartMoveRhythm(float bgmStartDspTime, float bpm)
    {
        _beatTimer.StartAtDspTime(bgmStartDspTime, bpm);
        _lastScheduledBeat = -999f;
        _isRunning = true;
    }

    public void Pause()
    {
        if (!_isRunning) return;
        _beatTimer.Pause();
        _isRunning = false;
    }

    public void Resume()
    {
        if (_isRunning) return;
        _beatTimer.Resume();
        _isRunning = true;
    }

    public void Stop()
    {
        _isRunning = false;
        _beatTimer.Stop();
        _lastScheduledBeat = -999f;
    }

    private void Update()
    {
        if (!_isRunning) return;

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