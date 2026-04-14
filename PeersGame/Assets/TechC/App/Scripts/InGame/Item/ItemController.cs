using System.Collections.Generic;
using UnityEngine;
using TechC.InGame.Log;
using TechC.InGame.Map;
using TechC.InGame.Player;

namespace TechC.InGame.Item
{
    public class ItemController : MonoBehaviour
    {
        [SerializeField] private GameObject _itemPrefab;
        [SerializeField, Min(0f)] private float _spawnInterval = 25f;
        [SerializeField, Min(1)] private int _maxItemCount = 5;
        [SerializeField, Min(0f)] private float _itemHeightOffset = 0.15f;
        [SerializeField, Min(1)] private int _healAmount = 1;

        private MapManager _mapManager;
        private float _spawnTimer;
        private readonly List<TileData> _itemTiles = new List<TileData>();

        private void Start()
        {
            _mapManager = InGame.Core.InGameManager.I?.MapManager;
            
            if (_mapManager == null)
            {
                CusLog.Warning("[ItemController] MapManager が取得できませんでした。");
                enabled = false;
                return;
            }

            if (_itemPrefab == null)
            {
                CusLog.Warning("[ItemController] ItemPrefab がアサインされていません。");
                enabled = false;
                return;
            }

            _spawnTimer = _spawnInterval;
            PlayerMover.OnTileReached += HandleTileReached;
        }

        private void OnDestroy()
        {
            PlayerMover.OnTileReached -= HandleTileReached;
        }

        private void Update()
        {
            if (_itemTiles.Count >= _maxItemCount) return;

            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer > 0f) return;

            _spawnTimer = _spawnInterval;
            TrySpawnRandomItem();
        }

        private void TrySpawnRandomItem()
        {
            if (_mapManager == null) return;

            Vector2Int currentPlayerPos = Vector2Int.one * -1; // マップ外の初期値
            if (PlayerController.I != null)
            {
                currentPlayerPos = PlayerController.I.CurrentGridPos;
            }

            var candidateTiles = _mapManager.GetWalkableEmptyTiles(currentPlayerPos);

            if (candidateTiles.Count == 0) return;

            var randomIndex = Random.Range(0, candidateTiles.Count);
            SpawnItemOnTile(candidateTiles[randomIndex]);
        }

        private void SpawnItemOnTile(TileData tile)
        {
            var tilePosition = tile.TileObject.transform.position;
            var itemPosition = tilePosition + Vector3.up * _itemHeightOffset;

            var itemObject = Instantiate(_itemPrefab, itemPosition, Quaternion.identity, tile.TileObject.transform);
            itemObject.name = "Item";

            tile.IsItem = true;
            tile.ItemObject = itemObject;
            _itemTiles.Add(tile);
            
            CusLog.Log($"[ItemController] アイテム生成 タイル={tile.GridPosition} (プレイヤー・敵を回避)");
        }

        private void HandleTileReached(TileData tile)
        {
            if (tile == null || !tile.IsItem) return;
            CollectItem(tile);
        }

        private void CollectItem(TileData tile)
        {
            if (tile.ItemObject != null)
            {
                Destroy(tile.ItemObject);
                tile.ItemObject = null;
            }

            tile.IsItem = false;
            _itemTiles.Remove(tile);
            ApplyItemEffect(tile);
        }

        private void ApplyItemEffect(TileData tile)
        {
            if (PlayerController.I == null)
            {
                CusLog.Warning("[ItemController] PlayerController が見つかりません。");
                return;
            }

            PlayerController.I.TakeHeal(_healAmount);
            CusLog.Log($"[ItemController] アイテム取得: {tile.GridPosition} / HP+{_healAmount}");
        }
    }
}