using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TechC.Core.Manager;
using TechC.InGame.Log;

namespace TechC.Audio
{
    /// <summary>
    /// オーディオ管理システム
    /// BGM・SEの再生、停止、ボリューム制御などを行う
    /// </summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [Header("設定")]
        [SerializeField] private AudioDataSO _audioDatabase;
        [SerializeField] private int _seSourceCount = 10; // SE用のAudioSource数

        [Header("初期ボリューム")]
        [Range(0f, 1f)]
        [SerializeField] private float _initialMasterVolume = 1f;
        [Range(0f, 1f)]
        [SerializeField] private float _initialBGMVolume = 0.8f;
        [Range(0f, 1f)]
        [SerializeField] private float _initialSEVolume = 1f;

        // DontDestroyOnLoad を使用（シーンを跨いで永続化）
        protected override bool UseDontDestroyOnLoad => true;

        // BGM用
        private AudioSource _bgmSource;
        private string _currentBGMName;
        private Coroutine _bgmFadeCoroutine;

        // SE用
        private List<AudioSource> _seSources = new List<AudioSource>();
        private int _seSourceIndex = 0;

        // ボリューム管理
        private float _masterVolume = 1f;
        private float _bgmVolume = 0.8f;
        private float _seVolume = 1f;

        // 一時停止状態
        private bool _isBGMPaused = false;
        private List<AudioSource> _pausedSESources = new List<AudioSource>();

        protected override void OnInitialize()
        {
            CusLog.Log("AudioManager", "AudioManager を初期化しました");

            // AudioSourceの作成
            SetupAudioSources();

            // 初期ボリューム設定
            _masterVolume = _initialMasterVolume;
            _bgmVolume = _initialBGMVolume;
            _seVolume = _initialSEVolume;

            // データベースチェック
            if (_audioDatabase == null)
            {
                CusLog.Error("AudioManager", "AudioDatabase が設定されていません！");
            }
        }

        /// <summary>
        /// AudioSourceのセットアップ
        /// </summary>
        private void SetupAudioSources()
        {
            // BGM用AudioSource
            _bgmSource = gameObject.AddComponent<AudioSource>();
            _bgmSource.loop = true;
            _bgmSource.playOnAwake = false;

            // SE用AudioSource
            for (int i = 0; i < _seSourceCount; i++)
            {
                AudioSource seSource = gameObject.AddComponent<AudioSource>();
                seSource.loop = false;
                seSource.playOnAwake = false;
                _seSources.Add(seSource);
            }
        }

        #region BGM制御

        /// <summary>
        /// BGMを再生
        /// </summary>
        public void PlayBGM(string bgmName, float fadeTime = 0f)
        {
            if (_audioDatabase == null)
            {
                CusLog.Error("AudioManager", "AudioDatabase が null です");
                return;
            }

            AudioData bgmData = _audioDatabase.GetAudioData(bgmName);
            if (bgmData == null)
            {
                CusLog.Error("AudioManager", $"BGM '{bgmName}' が見つかりません");
                return;
            }

            if (bgmData.type != AudioType.BGM)
            {
                CusLog.Warning("AudioManager", $"'{bgmName}' はBGMではありません");
                return;
            }

            // 既に同じBGMが再生中の場合はスキップ
            if (_currentBGMName == bgmName && _bgmSource.isPlaying)
            {
                return;
            }

            // フェード処理
            if (fadeTime > 0f)
            {
                if (_bgmFadeCoroutine != null)
                {
                    StopCoroutine(_bgmFadeCoroutine);
                }
                _bgmFadeCoroutine = StartCoroutine(FadeBGM(bgmData, fadeTime));
            }
            else
            {
                PlayBGMInternal(bgmData);
            }
        }

        /// <summary>
        /// BGMを内部的に再生
        /// </summary>
        private void PlayBGMInternal(AudioData bgmData)
        {
            _bgmSource.clip = bgmData.clip;
            _bgmSource.volume = bgmData.volume * _bgmVolume * _masterVolume;
            _bgmSource.pitch = bgmData.pitch;
            _bgmSource.loop = true;
            _bgmSource.Play();

            _currentBGMName = bgmData.name;
            _isBGMPaused = false;

            CusLog.Log("AudioManager", $"BGM '{bgmData.name}' を再生しました");
        }

        /// <summary>
        /// BGMをフェード付きで再生
        /// </summary>
        private IEnumerator FadeBGM(AudioData newBGM, float fadeTime)
        {
            float startVolume = _bgmSource.volume;

            // フェードアウト
            float elapsed = 0f;
            while (elapsed < fadeTime / 2f)
            {
                elapsed += Time.deltaTime;
                _bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / (fadeTime / 2f));
                yield return null;
            }

            // 新しいBGMを設定
            _bgmSource.clip = newBGM.clip;
            _bgmSource.pitch = newBGM.pitch;
            _bgmSource.Play();
            _currentBGMName = newBGM.name;

            // フェードイン
            elapsed = 0f;
            float targetVolume = newBGM.volume * _bgmVolume * _masterVolume;
            while (elapsed < fadeTime / 2f)
            {
                elapsed += Time.deltaTime;
                _bgmSource.volume = Mathf.Lerp(0f, targetVolume, elapsed / (fadeTime / 2f));
                yield return null;
            }

            _bgmSource.volume = targetVolume;
            _bgmFadeCoroutine = null;

            CusLog.Log("AudioManager", $"BGM '{newBGM.name}' をフェードイン再生しました");
        }

        /// <summary>
        /// BGMを停止
        /// </summary>
        public void StopBGM(float fadeTime = 0f)
        {
            if (!_bgmSource.isPlaying) return;

            if (fadeTime > 0f)
            {
                if (_bgmFadeCoroutine != null)
                {
                    StopCoroutine(_bgmFadeCoroutine);
                }
                _bgmFadeCoroutine = StartCoroutine(FadeOutBGM(fadeTime));
            }
            else
            {
                _bgmSource.Stop();
                _currentBGMName = null;
                CusLog.Log("AudioManager", "BGM を停止しました");
            }
        }

        /// <summary>
        /// BGMをフェードアウト
        /// </summary>
        private IEnumerator FadeOutBGM(float fadeTime)
        {
            float startVolume = _bgmSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeTime)
            {
                elapsed += Time.deltaTime;
                _bgmSource.volume = Mathf.Lerp(startVolume, 0f, elapsed / fadeTime);
                yield return null;
            }

            _bgmSource.Stop();
            _bgmSource.volume = startVolume;
            _currentBGMName = null;
            _bgmFadeCoroutine = null;

            CusLog.Log("AudioManager", "BGM をフェードアウト停止しました");
        }

        /// <summary>
        /// BGMを一時停止
        /// </summary>
        public void PauseBGM()
        {
            if (_bgmSource.isPlaying)
            {
                _bgmSource.Pause();
                _isBGMPaused = true;
                CusLog.Log("AudioManager", "BGM を一時停止しました");
            }
        }

        /// <summary>
        /// BGMを再開
        /// </summary>
        public void ResumeBGM()
        {
            if (_isBGMPaused)
            {
                _bgmSource.UnPause();
                _isBGMPaused = false;
                CusLog.Log("AudioManager", "BGM を再開しました");
            }
        }

        /// <summary>
        /// BGMが再生中かチェック
        /// </summary>
        public bool IsPlayingBGM()
        {
            return _bgmSource.isPlaying;
        }

        /// <summary>
        /// 現在のBGM名を取得
        /// </summary>
        public string GetCurrentBGMName()
        {
            return _currentBGMName;
        }

        #endregion

        #region SE制御

        /// <summary>
        /// SEを再生
        /// </summary>
        public void PlaySE(string seName, bool loop = false, float pitch = 1f)
        {
            if (_audioDatabase == null)
            {
                CusLog.Error("AudioManager", "AudioDatabase が null です");
                return;
            }

            AudioData seData = _audioDatabase.GetAudioData(seName);
            if (seData == null)
            {
                CusLog.Error("AudioManager", $"SE '{seName}' が見つかりません");
                return;
            }

            if (seData.type != AudioType.SE)
            {
                CusLog.Warning("AudioManager", $"'{seName}' はSEではありません");
                return;
            }

            PlaySEInternal(seData, loop, pitch);
        }

        /// <summary>
        /// SEを内部的に再生
        /// </summary>
        private void PlaySEInternal(AudioData seData, bool loop, float pitch)
        {
            // 利用可能なAudioSourceを取得
            AudioSource seSource = GetAvailableSESource();

            seSource.clip = seData.clip;
            seSource.volume = seData.volume * _seVolume * _masterVolume;
            seSource.pitch = pitch != 1f ? pitch : seData.pitch;
            seSource.loop = loop || seData.loop;
            seSource.spatialBlend = seData.spatialBlend;
            seSource.Play();

            CusLog.Log("AudioManager", $"SE '{seData.name}' を再生しました");
        }

        /// <summary>
        /// 3D位置指定でSEを再生
        /// </summary>
        public void PlaySE(string seName, Vector3 position, float pitch = 1f)
        {
            if (_audioDatabase == null)
            {
                CusLog.Error("AudioManager", "AudioDatabase が null です");
                return;
            }

            AudioData seData = _audioDatabase.GetAudioData(seName);
            if (seData == null)
            {
                CusLog.Error("AudioManager", $"SE '{seName}' が見つかりません");
                return;
            }

            // 一時的なGameObjectを作成して3D音源として再生
            GameObject tempGO = new GameObject($"TempSE_{seName}");
            tempGO.transform.position = position;
            AudioSource tempSource = tempGO.AddComponent<AudioSource>();

            tempSource.clip = seData.clip;
            tempSource.volume = seData.volume * _seVolume * _masterVolume;
            tempSource.pitch = pitch != 1f ? pitch : seData.pitch;
            tempSource.spatialBlend = 1f; // 完全3D
            tempSource.Play();

            // 再生終了後に削除
            Destroy(tempGO, seData.clip.length);

            CusLog.Log("AudioManager", $"SE '{seData.name}' を3D再生しました");
        }

        /// <summary>
        /// 利用可能なSE用AudioSourceを取得
        /// </summary>
        private AudioSource GetAvailableSESource()
        {
            // 再生していないソースを探す
            for (int i = 0; i < _seSources.Count; i++)
            {
                if (!_seSources[i].isPlaying)
                {
                    return _seSources[i];
                }
            }

            // 全て使用中の場合はラウンドロビン方式
            _seSourceIndex = (_seSourceIndex + 1) % _seSources.Count;
            return _seSources[_seSourceIndex];
        }

        /// <summary>
        /// 全てのSEを停止
        /// </summary>
        public void StopAllSE()
        {
            foreach (var seSource in _seSources)
            {
                seSource.Stop();
            }
            CusLog.Log("AudioManager", "全てのSEを停止しました");
        }

        /// <summary>
        /// 全てのSEを一時停止
        /// </summary>
        public void PauseAllSE()
        {
            _pausedSESources.Clear();
            foreach (var seSource in _seSources)
            {
                if (seSource.isPlaying)
                {
                    seSource.Pause();
                    _pausedSESources.Add(seSource);
                }
            }
            CusLog.Log("AudioManager", "全てのSEを一時停止しました");
        }

        /// <summary>
        /// 全てのSEを再開
        /// </summary>
        public void ResumeAllSE()
        {
            foreach (var seSource in _pausedSESources)
            {
                seSource.UnPause();
            }
            _pausedSESources.Clear();
            CusLog.Log("AudioManager", "全てのSEを再開しました");
        }

        #endregion

        #region ボリューム制御

        /// <summary>
        /// マスターボリュームを設定
        /// </summary>
        public void SetMasterVolume(float volume)
        {
            _masterVolume = Mathf.Clamp01(volume);
            UpdateAllVolumes();
            CusLog.Log("AudioManager", $"マスターボリューム: {_masterVolume:F2}");
        }

        /// <summary>
        /// BGMボリュームを設定
        /// </summary>
        public void SetBGMVolume(float volume)
        {
            _bgmVolume = Mathf.Clamp01(volume);
            UpdateBGMVolume();
            CusLog.Log("AudioManager", $"BGMボリューム: {_bgmVolume:F2}");
        }

        /// <summary>
        /// SEボリュームを設定
        /// </summary>
        public void SetSEVolume(float volume)
        {
            _seVolume = Mathf.Clamp01(volume);
            UpdateSEVolume();
            CusLog.Log("AudioManager", $"SEボリューム: {_seVolume:F2}");
        }

        /// <summary>
        /// 全てのボリュームを更新
        /// </summary>
        private void UpdateAllVolumes()
        {
            UpdateBGMVolume();
            UpdateSEVolume();
        }

        /// <summary>
        /// BGMボリュームを更新
        /// </summary>
        private void UpdateBGMVolume()
        {
            if (_bgmSource != null && _bgmSource.clip != null)
            {
                AudioData bgmData = _audioDatabase.GetAudioData(_currentBGMName);
                if (bgmData != null)
                {
                    _bgmSource.volume = bgmData.volume * _bgmVolume * _masterVolume;
                }
            }
        }

        /// <summary>
        /// SEボリュームを更新
        /// </summary>
        private void UpdateSEVolume()
        {
            // 再生中のSEのボリュームは次回再生時に反映
            // リアルタイム更新したい場合はここで実装
        }

        /// <summary>
        /// マスターボリュームを取得
        /// </summary>
        public float GetMasterVolume() => _masterVolume;

        /// <summary>
        /// BGMボリュームを取得
        /// </summary>
        public float GetBGMVolume() => _bgmVolume;

        /// <summary>
        /// SEボリュームを取得
        /// </summary>
        public float GetSEVolume() => _seVolume;

        #endregion

        #region ユーティリティ

        /// <summary>
        /// 全ての音を停止
        /// </summary>
        public void StopAll()
        {
            StopBGM();
            StopAllSE();
            CusLog.Log("AudioManager", "全ての音を停止しました");
        }

        /// <summary>
        /// 全ての音を一時停止
        /// </summary>
        public void PauseAll()
        {
            PauseBGM();
            PauseAllSE();
            CusLog.Log("AudioManager", "全ての音を一時停止しました");
        }

        /// <summary>
        /// 全ての音を再開
        /// </summary>
        public void ResumeAll()
        {
            ResumeBGM();
            ResumeAllSE();
            CusLog.Log("AudioManager", "全ての音を再開しました");
        }

        #endregion

        protected override void OnRelease()
        {
            StopAll();
        }
    }
}