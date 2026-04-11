using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace TechC.InGame.ObjectPool
{
    /// <summary>
    /// オブジェクトプール
    /// </summary>
    public class ObjectPoolManager : MonoBehaviour
    {
        [Header("Object Pool Settings")]
        [SerializeField] private List<ObjectPoolItem> _poolItems = new List<ObjectPoolItem>();
#if UNITY_EDITOR
        [SerializeField] private bool _autoExpand = true;
        [SerializeField] private int _expandSize = 5;
        
#endif

        private Dictionary<GameObject, Queue<GameObject>> _objectPools = new Dictionary<GameObject, Queue<GameObject>>();

        private Dictionary<string, GameObject> _prefabNameToOriginalPrefab = new Dictionary<string, GameObject>();

        private Dictionary<GameObject, ObjectPoolItem> _instanceToPoolItemMap = new Dictionary<GameObject, ObjectPoolItem>();

        public static ObjectPoolManager Instance;
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;

            InitializeAllPools();
        }


        /// <summary>
        /// すべてのプールを初期化します
        /// </summary>
        private void InitializeAllPools()
        {
            if (_poolItems == null || _poolItems.Count == 0)
            {
                Debug.LogWarning("Object Poolの初期化が不足しています。プールリストを設定してください。");
                return;
            }

            foreach (var poolItem in _poolItems)
            {
                if (poolItem.Prefab == null)
                {
                    Debug.LogError($"プール項目 '{poolItem.Name}' のプレハブがnullです。");
                    continue;
                }

                if (poolItem.Parent == null)
                {
                    poolItem.Parent = this.gameObject.transform;
                }

                InitializePool(poolItem);
            }
        }

        /// <summary>
        /// 指定したプレハブ用のプールを初期化します
        /// </summary>
        /// <param name="poolItem">初期化するプール項目</param>
        private void InitializePool(ObjectPoolItem poolItem)
        {
            if (poolItem.Prefab == null)
            {
                Debug.LogError("プレハブがnullのプールを初期化できません。");
                return;
            }

            if (!_objectPools.ContainsKey(poolItem.Prefab))
            {
                _objectPools[poolItem.Prefab] = new Queue<GameObject>();
            }

            string prefabName = poolItem.Prefab.name.Replace("(Clone)", "").Trim();
            if (!_prefabNameToOriginalPrefab.ContainsKey(prefabName))
                _prefabNameToOriginalPrefab[prefabName] = poolItem.Prefab;

            for (int i = 0; i < poolItem.InitialSize; i++)
            {
                GameObject newObject = CreateNewInstance(poolItem);
                _objectPools[poolItem.Prefab].Enqueue(newObject);
            }
        }

        /// <summary>
        /// 新しいインスタンスを作成します
        /// </summary>
        /// <param name="poolItem">生成元となるプール項目</param>
        /// <returns>生成されたGameObject</returns>
        private GameObject CreateNewInstance(ObjectPoolItem poolItem)
        {
            if (poolItem.Prefab == null)
            {
                Debug.LogError("nullのプレハブからインスタンスを作成できません。");
                return null;
            }

            GameObject newObject = Instantiate(poolItem.Prefab);
            newObject.SetActive(false);

            if (poolItem.Parent != null)
            {
                newObject.transform.SetParent(poolItem.Parent.transform);
            }

            _instanceToPoolItemMap[newObject] = poolItem;
            return newObject;
        }

        /// <summary>
        /// プールにプレハブを追加します
        /// </summary>
        /// <param name="name">プール項目の名前</param>
        /// <param name="prefab">プールするプレハブ</param>
        /// <param name="parent">親オブジェクト</param>
        /// <param name="initialSize">初期サイズ</param>
        public void AddToPool(string name, GameObject prefab, Transform parent, int initialSize)
        {
            if (prefab == null)
            {
                Debug.LogError("nullのプレハブをプールに追加できません。");
                return;
            }

            // すでに同じプレハブがプールに存在するか確認
            if (_objectPools.ContainsKey(prefab))
            {
                Debug.LogWarning($"プレハブ '{prefab.name}' はすでにプールに追加されています。");
                return;
            }

            ObjectPoolItem newItem = new ObjectPoolItem(name, prefab, parent ? parent : this.gameObject.transform, initialSize);
            _poolItems.Add(newItem);
            InitializePool(newItem);
        }

        /// <summary>
        /// プレハブからオブジェクトを取得します
        /// </summary>
        /// <param name="prefab">取得したいオブジェクトのプレハブ</param>
        /// <returns>アクティブ化されたGameObject</returns>
        public GameObject GetObject(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("❌ [ObjectPool] nullのプレハブからオブジェクトを取得できません。");
                return null;
            }

            // まずInstanceIDで検索
            GameObject actualPrefab = prefab;

            // InstanceIDで見つからない場合、名前で検索
            if (!_objectPools.ContainsKey(prefab))
            {
                string prefabName = prefab.name.Replace("(Clone)", "").Trim();
                if (_prefabNameToOriginalPrefab.TryGetValue(prefabName, out GameObject foundPrefab))
                    actualPrefab = foundPrefab;
            }

            // プールが存在し、オブジェクトがある場合
            if (_objectPools.TryGetValue(actualPrefab, out Queue<GameObject> pool) && pool.Count > 0)
            {
                GameObject pooledObject = pool.Dequeue();

                // nullチェック（破棄されたオブジェクトの対応）
                if (pooledObject == null)
                {
                    // nullの場合は新しいインスタンスを作成
                    ObjectPoolItem poolItem = _poolItems.Find(item => item.Prefab == actualPrefab);
                    if (poolItem != null)
                    {
                        pooledObject = CreateNewInstance(poolItem);
                    }
                }

                pooledObject.SetActive(true);

                // IPoolable インターフェースをサポート
                var poolable = pooledObject.GetComponent<IPoolable>();
                poolable?.OnPoolGet();

                return pooledObject;
            }
            else
            {
                // 初期リストに含まれるプレハブか確認
                ObjectPoolItem poolItem = _poolItems.Find(item => item.Prefab == actualPrefab);
                if (poolItem != null)
                {
#if UNITY_EDITOR
                    if (_autoExpand)
                    {
                        // プールの自動拡張
                        ExpandPool(poolItem, _expandSize);
                    }
                    else
                    {
                        // 拡張しない場合は5個で拡張
                        ExpandPool(poolItem, 5);
                    }
#else
                    // プール自動拡張（リリースビルド用固定設定）
                    ExpandPool(poolItem, 5);
#endif

                    // 拡張後に再度オブジェクトを取得
                    if (_objectPools[actualPrefab].Count > 0)
                    {
                        GameObject pooledObject = _objectPools[actualPrefab].Dequeue();
                        pooledObject.SetActive(true);

                        // IPoolable インターフェースをサポート
                        var poolable = pooledObject.GetComponent<IPoolable>();
                        poolable?.OnPoolGet();

                        return pooledObject;
                    }

                    // 拡張しない場合または拡張後もプールが空の場合は新しいインスタンスを作成
                    GameObject newObject = CreateNewInstance(poolItem);
                    newObject.SetActive(true);
                    return newObject;
                }
                else
                {
                    // プールに登録されていないプレハブの場合はエラー
                    Debug.LogError($"❌ [ObjectPool] プレハブ '{actualPrefab.name}' はプールに登録されていません。poolItems リストに追加してください。");
                    return null;
                }
            }
        }

        /// <summary>
        /// プレハブからオブジェクトを取得し、指定した位置と回転を設定します
        /// </summary>
        /// <param name="prefab">取得したいオブジェクトのプレハブ</param>
        /// <param name="position">設定する位置</param>
        /// <param name="rotation">設定する回転</param>
        /// <returns>アクティブ化されたGameObject</returns>
        public GameObject GetObject(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            GameObject obj = GetObject(prefab);
            if (obj != null)
            {
                obj.transform.position = position;
                obj.transform.rotation = rotation;
            }
            return obj;
        }

        /// <summary>
        /// 名前で指定したプールからオブジェクトを取得します
        /// </summary>
        /// <param name="poolName">プール名</param>
        /// <returns>アクティブ化されたGameObject</returns>
        public GameObject GetObjectByName(string poolName)
        {
            if (string.IsNullOrEmpty(poolName))
            {
                Debug.LogError("プール名がnullまたは空です。");
                return null;
            }

            ObjectPoolItem poolItem = _poolItems.Find(item => item.Name == poolName);
            if (poolItem != null && poolItem.Prefab != null)
            {
                return GetObject(poolItem.Prefab);
            }

            // Debug.LogWarning($"名前 '{poolName}' のプールが見つかりません。");
            return null;
        }

        /// <summary>
        /// プールを指定したサイズに拡張します
        /// </summary>
        /// <param name="poolItem">拡張するプール項目</param>
        /// <param name="expandCount">拡張するオブジェクト数</param>
        private void ExpandPool(ObjectPoolItem poolItem, int expandCount)
        {
            if (poolItem == null || poolItem.Prefab == null)
            {
                Debug.LogError("無効なプール項目でプールを拡張できません。");
                return;
            }

            for (int i = 0; i < expandCount; i++)
            {
                GameObject newObject = CreateNewInstance(poolItem);
                _objectPools[poolItem.Prefab].Enqueue(newObject);
            }

            // Debug.Log($"プール '{poolItem.name}' を {expandCount} 個拡張しました。現在のサイズ: {objectPools[poolItem.prefab].Count}");
        }

        /// <summary>
        /// オブジェクトをプールに返却します
        /// </summary>
        /// <param name="obj">プールに返却するオブジェクト</param>
        public void ReturnObject(GameObject obj)
        {
            if (obj == null)
            {
                Debug.LogWarning("nullのオブジェクトをプールに返却できません。");
                return;
            }

            // IPoolable インターフェースをサポート
            var poolable = obj.GetComponent<IPoolable>();
            poolable?.OnPoolReturn();

            // オブジェクトをリセット（位置・回転・スケール）
            obj.transform.localPosition = Vector3.zero;
            obj.transform.localRotation = Quaternion.identity;
            obj.transform.localScale = Vector3.one;

            obj.SetActive(false);

            if (_instanceToPoolItemMap.TryGetValue(obj, out ObjectPoolItem poolItem))
            {
                if (poolItem.Parent != null)
                    obj.transform.SetParent(poolItem.Parent.transform);

                if (_objectPools.ContainsKey(poolItem.Prefab))
                {
                    _objectPools[poolItem.Prefab].Enqueue(obj);
                }
                else
                {
                    // プールが見つからない場合は新しく作成
                    _objectPools[poolItem.Prefab] = new Queue<GameObject>();
                    _objectPools[poolItem.Prefab].Enqueue(obj);
                }
            }
            else
            {
                Debug.LogWarning($"オブジェクト '{obj.name}' はプールに登録されていません。削除します。");
                Destroy(obj);
            }
        }

        /// <summary>
        /// すべてのプールの統計情報を取得します
        /// </summary>
        /// <returns>各プールのサイズを含む文字列</returns>
        public string GetPoolStats()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Object Pool 統計:");

            foreach (var poolItem in _poolItems)
            {
                if (poolItem.Prefab != null && _objectPools.ContainsKey(poolItem.Prefab))
                {
                    sb.AppendLine($"プール '{poolItem.Name}': {_objectPools[poolItem.Prefab].Count} オブジェクト");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// すべてのプールを空にします
        /// </summary>
        public void ClearAllPools()
        {
            foreach (var pool in _objectPools.Values)
            {
                while (pool.Count > 0)
                {
                    GameObject obj = pool.Dequeue();
                    if (obj != null)
                    {
                        Destroy(obj);
                    }
                }
            }

            _objectPools.Clear();
            _instanceToPoolItemMap.Clear();

            // プールを再初期化
            InitializeAllPools();
        }

        /// <summary>
        /// 自動でプールの配列内にアイテムをセットします
        /// </summary>
        /// <param name="item"></param>
        public void AddPoolItem(ObjectPoolItem item)
        {
            _poolItems.Add(item);
        }

        /// <summary>
        /// 全てのプール内の非アクティブオブジェクトに任意の処理を実行します
        /// </summary>
        /// <param name="action">処理するアクション。GameObjectを引数に取ります</param>
        public void ForEachInactiveInPool(Action<GameObject> action)
        {
            if (action == null) return;

            foreach (var poolPair in _objectPools)
            {
                foreach (var obj in poolPair.Value)
                {
                    if (obj != null && !obj.activeInHierarchy)
                    {
                        action(obj);
                    }
                }
            }
        }

        /// <summary>
        /// このObjectPoolが指定したプレハブ名をサポートしているかチェック
        /// </summary>
        /// <param name="prefabName">チェックしたいプレハブ名</param>
        /// <returns>サポートしている場合はtrue</returns>
        public bool HasPrefabWithName(string prefabName)
        {
            return _poolItems.Exists(item => item.Prefab != null && item.Prefab.name == prefabName);
        }

        /// <summary>
        /// このObjectPoolがエフェクト系のオブジェクトをサポートしているかチェック
        /// </summary>
        /// <returns>エフェクト系のオブジェクトをサポートしている場合はtrue</returns>
        public bool IsEffectPool()
        {
            return _poolItems.Exists(item =>
                item.Name.Contains("Effect") ||
                (item.Prefab != null && item.Prefab.name.Contains("Effect"))
            );
        }

        /// <summary>
        /// このObjectPoolがプロジェクタイル系のオブジェクトをサポートしているかチェック
        /// </summary>
        /// <returns>プロジェクタイル系のオブジェクトをサポートしている場合はtrue</returns>
        public bool IsProjectilePool()
        {
            return _poolItems.Exists(item =>
                item.Name.Contains("Projectile") ||
                (item.Prefab != null && item.Prefab.name.Contains("Projectile"))
            );
        }

    }
}