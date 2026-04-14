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

        // ★判定管理用のリスト（生成された順に格納）
        private List<NoteController> _activeNoteList = new List<NoteController>();
        private int _index = 0;

        // ★判定の閾値（ビート単位：曲の速さに依存しないため正確）
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

            // 1. ノーツの生成処理
            while (_index < _notes.Count && _notes[_index].SpawnBeat <= currentBeat)
            {
                SpawnNote(_notes[_index]);
                _index++;
            }

            // 2. 入力検知と判定処理
            if (GetActionInput())
            {
                JudgeClosestNote(currentBeat);
            }

            // 3. 通り過ぎたノーツの自動Missチェック
            CheckAutoMiss(currentBeat);
        }

        /// <summary>
        /// マウス左クリックまたはスペースキーの入力を取得
        /// </summary>
        private bool GetActionInput()
        {
            var kb = Keyboard.current;
            var mouse = Mouse.current;

            bool spacePressed = kb != null && kb.spaceKey.wasPressedThisFrame;
            bool leftClickPressed = mouse != null && mouse.leftButton.wasPressedThisFrame;

            return spacePressed || leftClickPressed;
        }

        /// <summary>
        /// 最も判定ラインに近いノーツに対して判定を行う
        /// </summary>
        private void JudgeClosestNote(float currentBeat)
        {
            if (_activeNoteList.Count == 0) return;

            // リストの先頭（一番古いノーツ）がターゲット
            var targetNote = _activeNoteList[0];

            // NoteDataにTargetBeat（判定ラインに重なる拍）がある前提
            float diff = Mathf.Abs(targetNote.Data.TargetBeat - currentBeat);

            if (diff <= _perfectThreshold) ExecuteJudge(targetNote, "Perfect");
            else if (diff <= _greatThreshold) ExecuteJudge(targetNote, "Great");
            else if (diff <= _missThreshold) ExecuteJudge(targetNote, "Good");
            // 閾値より遠い場合は「空振り」として無視、もしくはMissにする
        }

        /// <summary>
        /// 一定以上通り過ぎたノーツをMissとして処理
        /// </summary>
        private void CheckAutoMiss(float currentBeat)
        {
            if (_activeNoteList.Count == 0) return;

            // ターゲット拍を一定以上（Miss閾値）超えたら自動回収
            if (currentBeat - _activeNoteList[0].Data.TargetBeat > _missThreshold)
            {
                ExecuteJudge(_activeNoteList[0], "Miss");
            }
        }

        /// <summary>
        /// 判定を確定させ、結果をログ出力および各マネージャーへ通知する
        /// </summary>
        /// <param name="note">対象のノーツ</param>
        /// <param name="rating">判定文字列 (Perfect, Great, Good, Miss)</param>
        private void ExecuteJudge(NoteController note, string rating)
        {
            if (note == null || BeatTimer.Instance == null) return;

            // 1. ログ出力（デバッグ用）
            float diff = Mathf.Abs(note.Data.TargetBeat - BeatTimer.Instance.GetCurrentBeat());
            CusLog.Log($"[Judge] {rating}! (Beat Diff: {diff:F3})");

            // 2. BattleManager への通知（ステップ5：ダメージ処理の窓口）
            // BattleManager 側に判定に応じた処理メソッドがある前提
            if (BattleManager.I != null)
            {
                // 例: BattleManager.I.ProcessBattleResult(rating, note.Data.Type);
            }

            // 3. ノーツ側にお片付けを指示（エフェクト再生とプール返却）
            bool isMiss = (rating == "Miss");
            note.OnJudged(isMiss);

            // 4. 管理リストから削除
            _activeNoteList.Remove(note);
        }

        private void SpawnNote(NoteData data)
        {
            GameObject prefab = (data.Type == NoteType.Attack) ? _attackNotePrefab : _defenseNotePrefab;
            GameObject obj = ObjectPoolManager.Instance.GetObject(prefab);

            var controller = obj.GetComponent<NoteController>();
            controller.Initialize(data);

            // リストに登録
            _activeNoteList.Add(controller);
        }

        /// <summary>
        /// ノーツをリストから除外し、非アクティブにする（Reset時などに使用）
        /// </summary>
        private void RemoveNote(NoteController note)
        {
            if (note == null) return;

            if (note.gameObject.activeSelf)
            {
                note.OnJudged(true);
            }

            if (_activeNoteList.Contains(note))
            {
                _activeNoteList.Remove(note);
            }
        }

        public void ResetSpawner()
        {
            _index = 0;

            // 画面上の全ノーツを非アクティブ化してリストをクリア
            foreach (var note in _activeNoteList)
            {
                if (note != null) note.gameObject.SetActive(false);
            }
            _activeNoteList.Clear();

            // 念のため子要素に残っているものも掃除
            var children = GetComponentsInChildren<NoteController>();
            foreach (var child in children) child.gameObject.SetActive(false);

            CusLog.Log("NoteSpawner: 判定リストとインデックスをリセットしました。");
        }
    }
}