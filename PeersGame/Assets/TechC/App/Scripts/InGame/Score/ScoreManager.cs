namespace TechC.InGame.Score
{
    /// <summary>
    /// インゲーム中のスコアを管理するクラス
    /// </summary>
    public class ScoreManager
    {
        private readonly ScoreDataSO _scoreData;

        /// <summary>現在のスコア</summary>
        public int Score { get; private set; }

        public ScoreManager(ScoreDataSO scoreData)
        {
            _scoreData = scoreData;
        }

        /// <summary>スコアを0にリセットする</summary>
        public void Reset() => Score = 0;

        /// <summary>移動成功時にスコアを加算する</summary>
        public void AddMoveSuccessScore() => Score += _scoreData.MoveSuccessScore;

        /// <summary>敵を倒した時にスコアを加算する</summary>
        public void AddEnemyDefeatedScore() => Score += _scoreData.EnemyDefeatedScore;

        /// <summary>攻撃成功時にスコアを加算する</summary>
        public void AddAttackSuccessScore() => Score += _scoreData.AttackSuccessScore;

        /// <summary>防御成功時にスコアを加算する</summary>
        public void AddDefenseSuccessScore() => Score += _scoreData.DefenseSuccessScore;
    }
}
