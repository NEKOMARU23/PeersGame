using UnityEngine;
using TechC.InGame.ObjectPool;
using TechC.Audio;

namespace TechC.InGame.Notes
{
    public class NoteController : MonoBehaviour
    {
        public NoteData Data => _data;
        private NoteData _data;
        
        private bool _isActive = false;

        [Header("Settings")]
        [SerializeField] private float _judgeX = 0f;
        // ★ _startX フィールドを削除、または初期化時に動的に取得するように変更
        private float _dynamicStartX; 

        [Header("Effects (Optional)")]
        [SerializeField] private ObjectPoolManager _effectPool;
        [SerializeField] private GameObject _attackEffect;
        [SerializeField] private GameObject _defenseEffect;

        public void Initialize(NoteData data)
        {
            _data = data;
            _isActive = true;
            
            // ★修正ポイント：
            // Spawnerが設定してくれた「現在の位置」をスタート地点として記録する
            // これにより、Spawner側の SpawnPoint を動かせば連動するようになります
            _dynamicStartX = transform.position.x;

            if (_effectPool == null)
            {
                _effectPool = ObjectPoolManager.Instance;
            }
        }

        private void Update()
        {
            if (!_isActive || _data == null || BeatTimer.Instance == null) return;

            float currentBeat = BeatTimer.Instance.GetCurrentBeat();
            
            float duration = _data.TargetBeat - _data.SpawnBeat;
            if (duration <= 0) return;

            float progress = (currentBeat - _data.SpawnBeat) / duration;

            float x = _dynamicStartX + (_judgeX - _dynamicStartX) * progress;

            transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }

        public void OnJudged(bool isMiss)
        {
            if (!_isActive) return;
            if (!isMiss) PlayHitEffect();

            _isActive = false;
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnObject(gameObject);
            }
        }

        private void PlayHitEffect()
        {
            GameObject effectPrefab = (_data.Type == NoteType.Attack) ? _attackEffect : _defenseEffect;
            if (effectPrefab != null && _effectPool != null)
            {
                GameObject effect = _effectPool.GetObject(effectPrefab);
                if (effect != null)
                {
                    effect.transform.position = transform.position;
                }
            }
        }
    }
}