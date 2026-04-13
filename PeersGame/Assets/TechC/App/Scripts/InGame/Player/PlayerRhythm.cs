using UnityEngine;
using UnityEngine.InputSystem;
using TechC.Rhythm;
using TechC.InGame.Log;
using TechC.InGame.Core;
using TechC.InGame.Score;

namespace TechC.InGame.Player
{
    public class PlayerRhythm : MonoBehaviour
    {
        [SerializeField] private MoveRhythmController _moveRhythmController;
        [SerializeField, Min(0f)] private float _timingWindowSec = 0.2f;
        [SerializeField, Min(1)] private int _timingMissDamage = 1;

        private void Update()
        {
            if (!HasMoveInputThisFrame()) return;

            if (_moveRhythmController == null)
            {
                CusLog.Warning("[PlayerRhyfhm] 判定スキップ: MoveRhythmController が未設定");
                return;
            }

            if (!_moveRhythmController.IsRunning)
            {
                CusLog.Warning("[PlayerRhyfhm] 判定スキップ: MoveRhythmController が未開始 (StartMoveRhythm 未呼び出し)");
                return;
            }

            if (IsTimingMiss())
            {
                CusLog.Log("[PlayerRhyfhm] タイミング失敗");
                PlayerController.I?.TakeDamage(_timingMissDamage);
                InGameManager.I?.ScoreManager.SubMoveFailScore();
                return;
            }

            CusLog.Log("[PlayerRhyfhm] タイミング成功");
            InGameManager.I?.ScoreManager.AddMoveSuccessScore();
        }

        private bool HasMoveInputThisFrame()
        {
            var kb = Keyboard.current;
            if (kb == null) return false;

            return kb.wKey.wasPressedThisFrame
                || kb.aKey.wasPressedThisFrame
                || kb.sKey.wasPressedThisFrame
                || kb.dKey.wasPressedThisFrame;
        }

        private bool IsTimingMiss()
        {
            if (_moveRhythmController == null) return false;

            return !_moveRhythmController.IsInputWithinBeatWindow(_timingWindowSec);
        }
    }
}
