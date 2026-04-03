using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace TechC.Audio
{
    /// <summary>
    /// オーディオの種類
    /// </summary>
    public enum AudioType
    {
        BGM,
        SE
    }

    /// <summary>
    /// 個別のオーディオデータ
    /// </summary>
    [System.Serializable]
    public class AudioData
    {
        [Header("基本設定")]
        public string Name;
        public AudioType Type;
        public AudioClip Clip;

        [Header("音量設定")]
        [Range(0f, 1f)]
        public float Volume = 1f;

        [Header("詳細設定")]
        public bool Loop = false;
        
        [Range(-3f, 3f)]
        public float Pitch = 1f;

        [Range(0f, 1f)]
        public float SpatialBlend = 0f; // 0 = 2D, 1 = 3D
    }

    /// <summary>
    /// オーディオデータを管理するScriptableObject
    /// </summary>
    [CreateAssetMenu(fileName = "AudioDataSO", menuName = "Audio/AudioDataSO")]
    public class AudioDataSO : ScriptableObject
    {
        [Header("オーディオリスト")]
        [SerializeField] private List<AudioData> _audioList = new List<AudioData>();

        public List<AudioData> AudioList => _audioList;

        /// <summary>
        /// 名前でオーディオデータを取得
        /// </summary>
        public AudioData GetAudioData(string audioName)
        {
            return _audioList.FirstOrDefault(data => data.Name == audioName);
        }

        /// <summary>
        /// タイプでオーディオデータのリストを取得
        /// </summary>
        public List<AudioData> GetAudioDataByType(AudioType type)
        {
            return _audioList.Where(data => data.Type == type).ToList();
        }

        /// <summary>
        /// BGMのリストを取得
        /// </summary>
        public List<AudioData> GetBGMList()
        {
            return GetAudioDataByType(AudioType.BGM);
        }

        /// <summary>
        /// SEのリストを取得
        /// </summary>
        public List<AudioData> GetSEList()
        {
            return GetAudioDataByType(AudioType.SE);
        }

        /// <summary>
        /// オーディオデータが存在するかチェック
        /// </summary>
        public bool HasAudioData(string audioName)
        {
            return _audioList.Any(data => data.Name == audioName);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor用：重複チェック
        /// </summary>
        private void OnValidate()
        {
            // 重複する名前をチェック
            var duplicates = _audioList
                .GroupBy(data => data.Name)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count > 0)
                Debug.LogWarning($"[AudioDatabase] 重複する名前が見つかりました: {string.Join(", ", duplicates)}");

            // 空のクリップをチェック
            var emptyClips = _audioList
                .Where(data => data.Clip == null)
                .Select(data => data.Name)
                .ToList();

            if (emptyClips.Count > 0)
                Debug.LogWarning($"[AudioDatabase] AudioClipが設定されていません: {string.Join(", ", emptyClips)}");
        }
#endif
    }
}