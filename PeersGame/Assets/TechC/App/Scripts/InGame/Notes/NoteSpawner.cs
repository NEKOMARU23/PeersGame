using UnityEngine;
using System.Collections.Generic;
using TechC.Audio;
using TechC.InGame.ObjectPool;
using TechC.Core.Manager;
using TechC.InGame.Log;

namespace TechC.InGame.Notes
{
    public class NoteSpawner : Singleton<NoteSpawner>
    {
        [SerializeField] private List<NoteData> _notes;
        [SerializeField] private GameObject _attackNotePrefab;
        [SerializeField] private GameObject _defenseNotePrefab;

        private int _index = 0;

        protected override bool UseDontDestroyOnLoad => false;

        protected override bool DestroyTargetGameObject => true;

        private void Update()
        {
            // インスタンスが有効かチェックする IsValid() を活用しても良いですね
            if (BeatTimer.Instance == null) return;

            float currentBeat = BeatTimer.Instance.GetCurrentBeat();

            while (_index < _notes.Count && _notes[_index].SpawnBeat <= currentBeat)
            {
                SpawnNote(_notes[_index]);
                _index++;
            }
        }

        /// <summary>
        /// 戦闘開始時などに呼び出して、譜面再生位置をリセットする
        /// </summary>
        public void ResetSpawner()
        {
            _index = 0;

            var activeNotes = GetComponentsInChildren<NoteController>();

            foreach (var note in activeNotes)
            {
                note.gameObject.SetActive(false);
            }

            CusLog.Log($"NoteSpawner: {activeNotes.Length} 個のノーツを回収し、インデックスをリセットしました。");
        }
        private void SpawnNote(NoteData data)
        {
            GameObject prefab = (data.Type == NoteType.Attack)
                ? _attackNotePrefab
                : _defenseNotePrefab;

            GameObject obj = ObjectPoolManager.Instance.GetObject(prefab);
            var controller = obj.GetComponent<NoteController>();
            controller.Initialize(data);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}