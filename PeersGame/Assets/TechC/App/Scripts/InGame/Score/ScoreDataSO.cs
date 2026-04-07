using UnityEngine;

namespace TechC.InGame.Score
{
    /// <summary>
    /// スコアの加算量を定義するScriptableObject
    /// Inspectorから各アクションのスコア値を設定する
    /// </summary>
    [CreateAssetMenu(fileName = "ScoreDataSO", menuName = "TechC/InGame/ScoreDataSO")]
    public class ScoreDataSO : ScriptableObject
    {
        [Header("スコア設定")]
        [SerializeField, Min(0), Tooltip("移動成功時の加算スコア")]   private int _moveSuccessScore    = 10;
        [SerializeField, Min(0), Tooltip("敵を倒した時の加算スコア")] private int _enemyDefeatedScore  = 100;
        [SerializeField, Min(0), Tooltip("攻撃成功時の加算スコア")]   private int _attackSuccessScore  = 20;
        [SerializeField, Min(0), Tooltip("防御成功時の加算スコア")]   private int _defenseSuccessScore = 30;

        /// <summary>移動成功時の加算スコア</summary>
        public int MoveSuccessScore    => _moveSuccessScore;

        /// <summary>敵を倒した時の加算スコア</summary>
        public int EnemyDefeatedScore  => _enemyDefeatedScore;

        /// <summary>攻撃成功時の加算スコア</summary>
        public int AttackSuccessScore  => _attackSuccessScore;

        /// <summary>防御成功時の加算スコア</summary>
        public int DefenseSuccessScore => _defenseSuccessScore;
    }
}
