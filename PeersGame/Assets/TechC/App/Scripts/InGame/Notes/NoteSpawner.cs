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
        [SerializeField] private List<NoteData> _notes;
        [SerializeField] private GameObject _attackNotePrefab;
        [SerializeField] private GameObject _defenseNotePrefab;

        private List<NoteController> _activeNoteList = new List<NoteController>();
        private int _index = 0;

        private int _successAttackCount = 0;
        private int _successDefenseCount = 0;

        [Header("Judge Thresholds (Beats)")]
        [SerializeField] private float _perfectThreshold = 0.1f;
        [SerializeField] private float _greatThreshold = 0.2f;
        [SerializeField] private float _missThreshold = 0.35f;

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
            bool spacePressed = kb != null && kb.spaceKey.wasPressedThisFrame;
            bool leftClickPressed = mouse != null && mouse.leftButton.wasPressedThisFrame;
            return spacePressed || leftClickPressed;
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
            if (note == null || BeatTimer.Instance == null) return;

            bool isSuccess = (rating == "Perfect" || rating == "Great" || rating == "Good");
            if (isSuccess)
            {
                if (note.Data.Type == NoteType.Attack) _successAttackCount++;
                else _successDefenseCount++;
            }

            CusLog.Log($"[Judge] {rating}! (Atk:{_successAttackCount} Def:{_successDefenseCount})");

            if (note.Data.IsResolutionTrigger)
            {
                ResolvePhrase();
            }

            note.OnJudged(rating == "Miss");
            _activeNoteList.Remove(note);
        }

        /// <summary>
        /// フレーズの精算結果を決定し、BattleManagerへ報告する
        /// </summary>
        private void ResolvePhrase()
        {
            bool attackSuccess = (_successAttackCount >= 5);
            bool defenseSuccess = (_successDefenseCount >= 5);

            CusLog.Log($"<color=yellow>[Phrase Resolve]</color> Attack:{attackSuccess} Defense:{defenseSuccess}");

            // ★BattleManagerへ結果を送る
            if (BattleManager.I != null)
            {
                BattleManager.I.OnPhraseResolved(attackSuccess, defenseSuccess);
            }

            // 次のフレーズのためにリセット
            _successAttackCount = 0;
            _successDefenseCount = 0;
        }

        private void SpawnNote(NoteData data)
        {
            GameObject prefab = (data.Type == NoteType.Attack) ? _attackNotePrefab : _defenseNotePrefab;
            GameObject obj = ObjectPoolManager.Instance.GetObject(prefab);
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

            var children = GetComponentsInChildren<NoteController>();
            foreach (var child in children) child.gameObject.SetActive(false);

            CusLog.Log("NoteSpawner: Reset Completed.");
        }
    }
}