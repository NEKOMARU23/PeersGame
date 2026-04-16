using System.Collections.Generic;
using UnityEngine;
using TechC.InGame.Log;
using TechC.InGame.Map;

namespace TechC.InGame.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private GameObject _enemyPrefab;
        [SerializeField] private Transform _enemyParent;

        [Header("Spawn Height Offset")]
        [SerializeField] private float _yOffset = 1.0f; // インスペクターから高さを調整可能

        /// <summary>
        /// プレイヤーの初期位置や既存のアイテム・敵を除いた空きタイルに敵を生成する
        /// </summary>
        public void SpawnEnemies(MapManager mapManager, int count, Vector2Int playerSpawnPos)
        {
            if (_enemyPrefab == null)
            {
                CusLog.Error("[EnemySpawner] EnemyPrefab が設定されていません。");
                return;
            }

            // MapManager 側の修正により、ここですでに「アイテムあり」や「敵あり」のタイルが除外される
            List<TileData> emptyTiles = mapManager.GetWalkableEmptyTiles(playerSpawnPos);

            // 生成数が候補数を超えないように調整
            int spawnCount = Mathf.Min(count, emptyTiles.Count);

            // リストをフィッシャー・イェーツの手法でシャッフル（ランダム抽出）
            for (int i = 0; i < emptyTiles.Count; i++)
            {
                TileData temp = emptyTiles[i];
                int randomIndex = Random.Range(i, emptyTiles.Count);
                emptyTiles[i] = emptyTiles[randomIndex];
                emptyTiles[randomIndex] = temp;
            }

            // 決定したタイルに敵を配置
            for (int i = 0; i < spawnCount; i++)
            {
                TileData targetTile = emptyTiles[i];
                Vector3 tilePos = targetTile.TileObject.transform.position;

                // めり込み防止のオフセットを適用
                Vector3 spawnPos = new Vector3(tilePos.x, tilePos.y + _yOffset, tilePos.z);

                GameObject enemyObj = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, _enemyParent);

                // 敵データにグリッド座標を教える
                EnemyDataOnTile enemyData = enemyObj.GetComponent<EnemyDataOnTile>();
                if (enemyData != null)
                {
                    enemyData.Setup(targetTile.GridPosition);
                }

                // タイル側の名簿に敵を登録
                targetTile.EnemyObject = enemyData;
            }

            CusLog.Log($"[EnemySpawner] {spawnCount} 体の敵を配置しました。(PlayerPos {playerSpawnPos} とアイテム位置を除外)");
        }
    }
}