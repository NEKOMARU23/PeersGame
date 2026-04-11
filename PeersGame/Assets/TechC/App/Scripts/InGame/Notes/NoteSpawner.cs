using UnityEngine;
using System.Collections.Generic;
using TechC.Audio;
using TechC.InGame.ObjectPool;

namespace TechC.InGame.Notes
{
    public class NoteSpawner : MonoBehaviour
    {
        [SerializeField] private List<NoteData> _notes; // 譜面データ
        [SerializeField] private GameObject _attackNotePrefab;
        [SerializeField] private GameObject _defenseNotePrefab;

        private int _index = 0;

        private void Update()
        {
            float currentBeat = BeatTimer.Instance.GetCurrentBeat();

            while (_index < _notes.Count && _notes[_index].SpawnBeat <= currentBeat)
            {
                SpawnNote(_notes[_index]);
                _index++;
            }
        }

        private void SpawnNote(NoteData data)
        {
            // ★ NoteType に応じて Prefab を選択
            GameObject prefab = (data.Type == NoteType.Attack)
                ? _attackNotePrefab
                : _defenseNotePrefab;

            // ★ ObjectPoolManager から取得
            GameObject obj = ObjectPoolManager.Instance.GetObject(prefab);

            // ★ NoteController を取得して初期化
            var controller = obj.GetComponent<NoteController>();
            controller.Initialize(data);
        }
    }
}