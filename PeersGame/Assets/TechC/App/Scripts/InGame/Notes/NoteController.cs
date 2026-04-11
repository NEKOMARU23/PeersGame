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

            if (diff <= _perfectRange)
            {
                Debug.Log("PERFECT!");
                GameObject attackEffect = _effectPool.GetObject(_attackEffect);
            }
            else if (diff <= _goodRange)
            {
                Debug.Log("GOOD!");
                GameObject attackEffect = _effectPool.GetObject(_attackEffect);
            }
            else
            {
                Debug.Log("MISS!");
            }

            _isActive = false;
            ObjectPoolManager.Instance.ReturnObject(gameObject);
        }
    }
}