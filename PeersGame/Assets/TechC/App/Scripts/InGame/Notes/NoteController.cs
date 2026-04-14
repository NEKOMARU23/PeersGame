using UnityEngine;
using TechC.InGame.ObjectPool;
using Unity.VisualScripting;
using TechC.InGame.Notes;

namespace TechC.InGame.Notes
{
    public class NoteController : MonoBehaviour
    {
        private NoteData _data;
        private bool _isActive = false;

        [SerializeField] private float _speed = 5f;     // 横に流れる速度
        [SerializeField] private float _judgeX = 0f;    // 画面中央の判定ライン
        [SerializeField] private float _perfectRange = 0.2f;
        [SerializeField] private float _goodRange = 0.5f;
        [SerializeField] private ObjectPoolManager _effectPool;
        private GameObject _attackEffect;
        private GameObject _deffenceEffct;

        public void Initialize(NoteData data)
        {
            _data = data;
            _isActive = true;
        }

        private void Update()
        {
            if (!_isActive) return;

            // 横方向に流れる（右→左）
            transform.Translate(Vector3.left * _speed * Time.deltaTime);

            // 判定ラインを通過したら判定
            if (transform.position.x <= _judgeX)
            {
                Judge();
            }
        }

        private void Judge()
        {
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
                GameObject effectToSpawn = (_data.Type == NoteType.Attack) ? _attackEffect : _deffenceEffct;

                if (effectToSpawn != null && _effectPool != null)
                {
                    GameObject effect = _effectPool.GetObject(effectToSpawn);
                    if (effect != null)
                    {
                        effect.transform.position = transform.position;
                    }
                }
                else
                {
                    Debug.LogWarning($"{_data.Type} のエフェクトプレハブが未設定です。生成をスキップします。");
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