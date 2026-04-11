using UnityEngine;

namespace TechC.Audio
{
    public class BeatTimer : MonoBehaviour
    {
        private float _bpm;
        private float _startDspTime;
        private float _beatOffset;

        private bool _isRunning = false;
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
 
        public void Start(float bpm, float delaySec = 0f)
        {
            _bpm = bpm;
            _startDspTime = (float)AudioSettings.dspTime + delaySec;
            _beatOffset = 0f;
            _isRunning = true;
        }

        public void StartAtDspTime(float dspTime, float bpm)
        {
            _bpm = bpm;
            _startDspTime = dspTime;
            _beatOffset = 0f;
            _isRunning = true;
        }

        public float GetCurrentBeat()
        {
            if (!_isRunning)
                return _pausedBeat;

            float dsp = (float)AudioSettings.dspTime;
            float sec = dsp - _startDspTime;
            return sec * _bpm / 60f + _beatOffset;
        }

        public float GetDspTimeForBeat(float beat)
        {
            return _startDspTime + ((beat - _beatOffset) * 60f / _bpm);
        }

        public void Pause()
        {
            if (!_isRunning) return;
            _pausedBeat = GetCurrentBeat();
            _isRunning = false;
        }

        public void Resume()
        {
            if (_isRunning) return;

            float dsp = (float)AudioSettings.dspTime;
            _startDspTime = dsp - (_pausedBeat * 60f / _bpm);
            _isRunning = true;
        }

        public void Stop()
        {
            _isRunning = false;
            _pausedBeat = 0f;
        }
    }
}