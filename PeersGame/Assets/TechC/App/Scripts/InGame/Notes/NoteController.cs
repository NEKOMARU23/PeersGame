using UnityEngine;
using TechC.InGame.ObjectPool;
using TechC.Audio;

namespace TechC.InGame.Notes
{

    /// <summary>
    /// ノーツの動きや判定を管理するクラス
    /// </summary>
    public class NoteController : MonoBehaviour
    {
        private NoteData _data;
        private bool _isActive = false;

        [Header("Settings")]
        [SerializeField] private float _judgeX = 0f;
        [SerializeField] private float _startX = 15f;
        [SerializeField] private float _perfectRange = 0.2f;
        [SerializeField] private float _goodRange = 0.5f;

        [Header("Effects (Optional)")]
        [SerializeField] private ObjectPoolManager _effectPool;
        [SerializeField] private GameObject _attackEffect;
        [SerializeField] private GameObject _defenseEffect;

        /// <summary>
        /// ノーツを初期化して出現させる
        /// </summary>
        /// <param name="data"></param>
        public void Initialize(NoteData data)
        {
            _data = data;
            _isActive = true;
            transform.position = new Vector3(_startX, 0, 0);

            if (_effectPool == null)
            {
                _effectPool = ObjectPoolManager.Instance;
            }

            Debug.Log($"Note Initialized: TargetBeat {data.TargetBeat}");
        }

        private void Update()
        {
            if (!_isActive || _data == null || BeatTimer.Instance == null) return;

            float currentBeat = BeatTimer.Instance.GetCurrentBeat();
            float duration = _data.TargetBeat - _data.SpawnBeat;
            float progress = (currentBeat - _data.SpawnBeat) / duration;

            float x = Mathf.Lerp(_startX, _judgeX, progress);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);

            if (currentBeat >= _data.TargetBeat + 0.1f)
            {
                Judge();
            }
        }

        /// <summary>
        /// 判定処理。成功ならエフェクトを出してスコア加算、失敗ならMISS表示など。
        /// </summary>
        public void Judge()
        {
            if (!_isActive) return;

            float diff = Mathf.Abs(transform.position.x - _judgeX);
            bool isSuccess = false;

            if (diff <= _perfectRange)
            {
                Debug.Log("<color=yellow>PERFECT!</color>");
                isSuccess = true;
            }
            else if (diff <= _goodRange)
            {
                Debug.Log("<color=green>GOOD!</color>");
                isSuccess = true;
            }
            else
            {
                Debug.Log("<color=red>MISS!</color>");
            }

            if (isSuccess)
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

            _isActive = false;

            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnObject(gameObject);
            }
        }
    }
}