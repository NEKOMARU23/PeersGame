using UnityEngine;
using UnityEngine.InputSystem;
using TechC.InGame.Core;
using System.Collections.Generic;
using TechC.Audio;
using TechC.InGame.ObjectPool;
using TechC.Core.Manager;
using TechC.InGame.Log;

namespace TechC.InGame.Notes
{
    public class NoteSpawner : Singleton<NoteSpawner>
    {
        [Header("Note Prefabs")]
        [SerializeField] private List<NoteData> _notes;
        [SerializeField] private GameObject _attackNotePrefab;
        [SerializeField] private GameObject _defenseNotePrefab;

        [Header("Spawn Settings")]
        // ★追加：ノーツが出現する基準位置。インスペクターのここをいじればノーツの位置が変わります。
        [SerializeField] private Transform _spawnPoint; 

        [Header("Judge Thresholds (Beats)")]
        [SerializeField] private float _perfectThreshold = 0.1f;
        [SerializeField] private float _greatThreshold = 0.2f;
        [SerializeField] private float _missThreshold = 0.35f;

        private List<NoteController> _activeNoteList = new List<NoteController>();
        private int _index = 0;

        private int _successAttackCount = 0;
        private int _successDefenseCount = 0;

        protected override bool UseDontDestroyOnLoad => false;
        protected override bool DestroyTargetGameObject => true;

        private void Update()
        {
            if (BeatTimer.Instance == null || !BeatTimer.Instance.IsRunning) return;

            float currentBeat = BeatTimer.Instance.GetCurrentBeat();

            while (_index < _notes.Count && _notes[_index].SpawnBeat <= currentBeat)
            {
                SpawnNote(_notes[_index]);
                _index++;
            }

            if (GetActionInput())
            {
                JudgeClosestNote(currentBeat);
            }

            CheckAutoMiss(currentBeat);
        }

        private bool GetActionInput()
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;
            return (kb != null && kb.spaceKey.wasPressedThisFrame) || (mouse != null && mouse.leftButton.wasPressedThisFrame);
        }

        private void JudgeClosestNote(float currentBeat)
        {
            if (_activeNoteList.Count == 0) return;

            var targetNote = _activeNoteList[0];
            float diff = Mathf.Abs(targetNote.Data.TargetBeat - currentBeat);

            if (diff <= _perfectThreshold) ExecuteJudge(targetNote, "Perfect");
            else if (diff <= _greatThreshold) ExecuteJudge(targetNote, "Great");
            else if (diff <= _missThreshold) ExecuteJudge(targetNote, "Good");
        }

        private void CheckAutoMiss(float currentBeat)
        {
            if (_activeNoteList.Count == 0) return;

            if (currentBeat - _activeNoteList[0].Data.TargetBeat > _missThreshold)
            {
                ExecuteJudge(_activeNoteList[0], "Miss");
            }
        }

        private void ExecuteJudge(NoteController note, string rating)
        {
            if (note == null) return;

            bool isSuccess = (rating == "Perfect" || rating == "Great" || rating == "Good");
            if (isSuccess)
            {
                if (note.Data.Type == NoteType.Attack) _successAttackCount++;
                else _successDefenseCount++;
            }

            if (note.Data.IsResolutionTrigger)
            {
                ResolvePhrase();
            }

            note.OnJudged(rating == "Miss");
            _activeNoteList.Remove(note);
        }

        private void ResolvePhrase()
        {
            bool attackSuccess = (_successAttackCount >= 5);
            bool defenseSuccess = (_successDefenseCount >= 5);

            if (BattleManager.I != null)
            {
                BattleManager.I.OnPhraseResolved(attackSuccess, defenseSuccess);
            }

            _successAttackCount = 0;
            _successDefenseCount = 0;
        }

        public void PrepareNextLoop()
        {
            _index = 0;
            foreach (var note in _activeNoteList)
            {
                if (note != null) note.gameObject.SetActive(false);
            }
            _activeNoteList.Clear();
            CusLog.Log("NoteSpawner: 次のターンの準備が完了しました。");
        }

        private void SpawnNote(NoteData data)
        {
            GameObject prefab = (data.Type == NoteType.Attack) ? _attackNotePrefab : _defenseNotePrefab;
            GameObject obj = ObjectPoolManager.Instance.GetObject(prefab);

            // ★修正ポイント：生成（プールから取得）した瞬間に、指定した座標へ移動させる
            if (_spawnPoint != null)
            {
                obj.transform.position = _spawnPoint.position;
                obj.transform.rotation = _spawnPoint.rotation;
            }
            else
            {
                // SpawnPointが未設定の場合はSpawner自身の位置に出す
                obj.transform.position = transform.position;
            }

            var controller = obj.GetComponent<NoteController>();
            controller.Initialize(data);
            _activeNoteList.Add(controller);
        }

        public void ResetSpawner()
        {
            _index = 0;
            _successAttackCount = 0;
            _successDefenseCount = 0;

            foreach (var note in _activeNoteList)
            {
                if (note != null) note.gameObject.SetActive(false);
            }
            _activeNoteList.Clear();
            CusLog.Log("NoteSpawner: 完全リセット完了。");
        }
    }
}