using UnityEngine;
using TechC.InGame.ObjectPool;
using TechC.Audio;

namespace TechC.InGame.Notes
{
    /// <summary>
    /// ノーツの動きやエフェクト演出を担当するクラス
    /// 判定ロジックは NoteSpawner が一括管理する
    /// </summary>
    public class NoteController : MonoBehaviour
    {
        // プロパティを公開して NoteSpawner から Data にアクセスできるようにする
        public NoteData Data => _data;
        private NoteData _data;
        
        private bool _isActive = false;

        [Header("Settings")]
        [SerializeField] private float _judgeX = 0f;
        [SerializeField] private float _startX = 15f;

        [Header("Effects (Optional)")]
        [SerializeField] private ObjectPoolManager _effectPool;
        [SerializeField] private GameObject _attackEffect;
        [SerializeField] private GameObject _defenseEffect;

        /// <summary>
        /// ノーツを初期化して出現させる
        /// </summary>
        public void Initialize(NoteData data)
        {
            _data = data;
            _isActive = true;
            transform.position = new Vector3(_startX, 0, 0);

            if (_effectPool == null)
            {
                _effectPool = ObjectPoolManager.Instance;
            }

            // Debug.Log($"Note Initialized: TargetBeat {data.TargetBeat}");
        }

        private void Update()
        {
            // 移動処理
            if (!_isActive || _data == null || BeatTimer.Instance == null) return;

            float currentBeat = BeatTimer.Instance.GetCurrentBeat();
            
            // 生成からターゲットまでの時間(拍)
            float duration = _data.TargetBeat - _data.SpawnBeat;
            if (duration <= 0) return;

            // 進捗率を計算 (0 = StartX, 1 = JudgeX)
            float progress = (currentBeat - _data.SpawnBeat) / duration;

            float x = Mathf.Lerp(_startX, _judgeX, progress);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);

            // 自己判定(Judge())は NoteSpawner 側で行うため、ここでは行わない
        }

        /// <summary>
        /// 判定成功時のエフェクト生成とオブジェクト回収
        /// </summary>
        /// <param name="isMiss">Miss判定の場合はエフェクトを出さない</param>
        public void OnJudged(bool isMiss)
        {
            if (!_isActive) return;

            if (!isMiss)
            {
                PlayHitEffect();
            }

            _isActive = false;

            // プールに返却
            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnObject(gameObject);
            }
        }

        /// <summary>
        /// 判定に合わせたエフェクトを再生
        /// </summary>
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