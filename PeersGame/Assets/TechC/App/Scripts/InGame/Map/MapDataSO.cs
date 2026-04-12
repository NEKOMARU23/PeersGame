using UnityEngine;

namespace TechC.InGame.Map
{
    /// <summary>
    /// マップの設定データを定義するScriptableObject
    /// Inspectorからマップの大きさや生成するPrefabを設定する
    /// </summary>
    [CreateAssetMenu(fileName = "MapDataSO", menuName = "TechC/InGame/MapDataSO")]
    public class MapDataSO : ScriptableObject
    {
        [Header("マップサイズ")]
        [SerializeField, Min(1), Tooltip("マップの縦のマス数")] private int _rows = 5;
        [SerializeField, Min(1), Tooltip("マップの横のマス数")] private int _columns = 5;

        [Header("タイル設定")]
        [SerializeField] private GameObject _tilePrefab;

        [Header("タイル間隔")]
        [SerializeField, Min(0.1f)] private float _tileSpacing = 1f;

        /// <summary>縦のマス数</summary>
        public int Rows => _rows;

        /// <summary>横のマス数</summary>
        public int Columns => _columns;

        /// <summary>生成するタイルのPrefab</summary>
        public GameObject TilePrefab => _tilePrefab;

        /// <summary>タイル間の間隔</summary>
        public float TileSpacing => _tileSpacing;
    }
}