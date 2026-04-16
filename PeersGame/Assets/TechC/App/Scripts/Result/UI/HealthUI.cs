using System.Collections.Generic;
using UnityEngine;

namespace TechC.InGame.UI
{
    public class HealthUI : MonoBehaviour
    {
        [SerializeField] private GameObject _heartPrefab;
        [SerializeField] private Transform _heartParent;
        private List<GameObject> _hearts = new List<GameObject>();

        // 最初にハートを多めに（最大HP分）作っておく
        public void Setup(int maxHp)
        {
            // 既にあったら一回消す
            foreach (var h in _hearts) if(h != null) Destroy(h);
            _hearts.Clear();

            for (int i = 0; i < maxHp; i++)
            {
                GameObject h = Instantiate(_heartPrefab, _heartParent);
                _hearts.Add(h);
            }
        }

        // 命令を受け取って、ハートの数を変える
        public void UpdateDisplay(int currentHp)
        {
            // 親をアクティブにする（死んだ後などに再表示するため）
            _heartParent.gameObject.SetActive(true);

            for (int i = 0; i < _hearts.Count; i++)
            {
                if (_hearts[i] != null)
                {
                    _hearts[i].SetActive(i < currentHp);
                }
            }
        }

        // 敵がいない時は隠す
        public void Hide()
        {
            _heartParent.gameObject.SetActive(false);
        }
    }
}