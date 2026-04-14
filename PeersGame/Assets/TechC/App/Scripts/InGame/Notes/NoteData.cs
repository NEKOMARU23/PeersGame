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

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="targetBeat">叩くべき拍</param>
        /// <param name="type">種類</param>
        /// <param name="preSpawnBeats">出現から判定まで何拍かけるか（デフォルト4拍）</param>
        public NoteData(float targetBeat, NoteType type, float preSpawnBeats = 4f)
        {
            TargetBeat = targetBeat;
            Type = type;
            // 指定された拍数分だけ前に出現させる
            SpawnBeat = targetBeat - preSpawnBeats;
        }
    }
}