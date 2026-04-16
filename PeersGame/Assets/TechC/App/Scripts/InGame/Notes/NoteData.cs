using System; 
using UnityEngine;

namespace TechC.InGame.Notes
{
    /// <summary>
    /// ノーツの種類（攻撃・防御）
    /// </summary>
    public enum NoteType
    {
        Attack,
        Defense
    }

    /// <summary>
    /// 1つのノーツの情報を保持するデータクラス
    /// </summary>
    [Serializable]
    public class NoteData
    {
        [Tooltip("判定ラインに重なる拍数")]
        public float TargetBeat;

        [Tooltip("画面に出現する拍数")]
        public float SpawnBeat;

        [Tooltip("ノーツのタイプ")]
        public NoteType Type;

        [Tooltip("フレーズの最後として精算処理を行うか")]
        public bool IsResolutionTrigger;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public NoteData(float targetBeat, NoteType type, float preSpawnBeats = 4f, bool isResolution = false)
        {
            TargetBeat = targetBeat;
            Type = type;
            SpawnBeat = targetBeat - preSpawnBeats;
            IsResolutionTrigger = isResolution;
        }
    }
}