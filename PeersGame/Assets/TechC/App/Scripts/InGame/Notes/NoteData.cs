using UnityEngine;

namespace TechC.InGame.Notes
{


    public enum NoteType
    {
        Attack,
        Defense
    }

    public class NoteData
    {
        public float TargetBeat;
        public float SpawnBeat;
        public NoteType Type;

        public NoteData(float targetBeat, NoteType type, float preSpawnBeats = 4f)
        {
            TargetBeat = targetBeat;
            Type = type;
            SpawnBeat = targetBeat - preSpawnBeats;
        }
    }

}