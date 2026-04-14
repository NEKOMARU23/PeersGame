using UnityEngine;
using TechC.InGame.ObjectPool;
using TechC.Audio;

namespace TechC.InGame.Notes
{
    public class NoteController : MonoBehaviour
    {
        private NoteData _data;
        private bool _isActive = false;

        [Header("Settings")]
        [SerializeField] private float _judgeX = 0f;        // 判定ラインのX座標
        [SerializeField] private float _startX = 15f;       // 出現時の右端のX座標
        [SerializeField] private float _perfectRange = 0.2f;
        [SerializeField] private float _goodRange = 0.5f;

        [Header("Effects (Optional)")]
        [SerializeField] private ObjectPoolManager _effectPool;
        [SerializeField] private GameObject _attackEffect;
        [SerializeField] private GameObject _deffenceEffct;

        /// <summary>
        /// ノーツ生成時（プールからの取得時）に呼ばれる初期化関数
        /// </summary>
        public void Initialize(NoteData data)
        {
            _data = data;
            
            // 1. 再利用のためにフラグを必ず true にする
            _isActive = true;

            // 2. 初期位置を出現ポイントにセット
            transform.position = new Vector3(_startX, 0, 0);

            // 3. エフェクトプールが未設定ならシングルトンから取得を試みる（予備策）
            if (_effectPool == null)
            {
                _effectPool = ObjectPoolManager.Instance;
            }

            Debug.Log($"Note Initialized: TargetBeat {data.TargetBeat}");
        }

        private void Update()
        {
            // アクティブでない、またはデータがない場合は何もしない
            if (!_isActive || _data == null || BeatTimer.Instance == null) return;

            // --- 拍数に基づいた位置計算 ---
            float currentBeat = BeatTimer.Instance.GetCurrentBeat();
            
            // 出現拍からターゲット拍までの総時間（拍単位）
            float duration = _data.TargetBeat - _data.SpawnBeat;
            
            // 現在の進捗度 (0.0 ～ 1.0)
            // もし SpawnBeat より前なら 0 以下、TargetBeat を過ぎれば 1 以上になる
            float progress = (currentBeat - _data.SpawnBeat) / duration;

            // 線形補間（Lerp）で X 座標を決定
            float x = Mathf.Lerp(_startX, _judgeX, progress);
            transform.position = new Vector3(x, transform.position.y, transform.position.z);

            // --- 自動判定処理 ---
            // ターゲットの拍数に到達したら Judge を実行してプールに戻る
            if (currentBeat >= _data.TargetBeat)
            {
                Judge();
            }
        }

        /// <summary>
        /// 判定処理。プレイヤーの入力や時間切れによって呼ばれる
        /// </summary>
        public void Judge()
        {
            // 二重判定を防止
            if (!_isActive) return;

            // 判定ラインとの距離で評価
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

            // エフェクト生成（プレハブが設定されている場合のみ）
            if (isSuccess)
            {
                GameObject effectPrefab = (_data.Type == NoteType.Attack) ? _attackEffect : _deffenceEffct;

                if (effectPrefab != null && _effectPool != null)
                {
                    GameObject effect = _effectPool.GetObject(effectPrefab);
                    if (effect != null)
                    {
                        effect.transform.position = transform.position;
                    }
                }
                else if (effectPrefab == null)
                {
                    // エフェクトがなくてもエラーにせずログに留める
                    Debug.LogWarning($"{_data.Type} のエフェクトプレハブが未設定です。");
                }
            }

            // 終了処理：フラグを下ろしてプールに返却
            _isActive = false;

            if (ObjectPoolManager.Instance != null)
            {
                ObjectPoolManager.Instance.ReturnObject(gameObject);
            }
        }
    }
}