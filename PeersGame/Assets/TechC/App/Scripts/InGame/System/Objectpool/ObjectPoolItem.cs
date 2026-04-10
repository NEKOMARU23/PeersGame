using System;
using UnityEngine;

namespace TechC.InGame.ObjectPool
{
    [Serializable]
    public class ObjectPoolItem
    {
        [Tooltip("プール識別用の名前")]
        public string Name;   // ← Manager が参照しているのはこれ

        [Tooltip("プールするプレハブ")]
        public GameObject Prefab;

        [Tooltip("生成されたオブジェクトを格納する親オブジェクト")]
        public Transform Parent;   // ← Transform に統一

        [Tooltip("初期プールサイズ")]
        [Range(0, 1000)]
        public int InitialSize = 5;

        public ObjectPoolItem() { }

        public ObjectPoolItem(string name, GameObject prefab, Transform parent, int initialSize)
        {
            this.Name = name;
            this.Prefab = prefab;
            this.Parent = parent;
            this.InitialSize = Mathf.Max(0, initialSize);
        }

        public bool IsValid()
        {
            return Prefab != null;
        }
    }
}
