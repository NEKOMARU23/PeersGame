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
        [SerializeField] private float _yOffset = 1.0f; 

        /// <summary>
        /// 空きタイルに敵を生成し、実際に生成された数を返す
        /// </summary>
        /// <returns>生成に成功した敵の数</returns>
        public int SpawnEnemies(MapManager mapManager, int count, Vector2Int playerSpawnPos)
        {
            if (_enemyPrefab == null)
            {
                CusLog.Error("[EnemySpawner] EnemyPrefab が設定されていません。");
                return 0;
            }

            // 空きタイルのリストを取得
            List<TileData> emptyTiles = mapManager.GetWalkableEmptyTiles(playerSpawnPos);

            // 生成数が候補数を超えないように調整
            int spawnCount = Mathf.Min(count, emptyTiles.Count);
            
            // ★追加：実際に生成に成功した数をカウントする
            int actualSuccessCount = 0;

            // リストをシャッフル（ランダム抽出）
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

                // オフセット適用
                Vector3 spawnPos = new Vector3(tilePos.x, tilePos.y + _yOffset, tilePos.z);

                GameObject enemyObj = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, _enemyParent);

                // 敵データにグリッド座標をセット
                EnemyDataOnTile enemyData = enemyObj.GetComponent<EnemyDataOnTile>();
                if (enemyData != null)
                {
                    enemyData.Setup(targetTile.GridPosition);
                    // タイル側の名簿に敵を登録
                    targetTile.EnemyObject = enemyData;
                    
                    // 生成成功カウントを加算
                    actualSuccessCount++;
                }
                else
                {
                    CusLog.Warning($"[EnemySpawner] 生成されたオブジェクトに EnemyDataOnTile が付与されていません。");
                }
            }

            CusLog.Log($"[EnemySpawner] {actualSuccessCount} 体の敵を配置しました。");
            
            // ★重要：呼び出し元の MapGenerator に実数を報告する
            return actualSuccessCount;
        }
    }
}