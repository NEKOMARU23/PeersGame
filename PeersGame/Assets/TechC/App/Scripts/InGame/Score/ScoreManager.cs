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

        /// <summary>移動失敗時にスコアを減算する</summary>
        public void SubMoveFailScore() => Score -= _scoreData.MoveFailScore;

        /// <summary>攻撃失敗時にスコアを減算する</summary>
        public void SubAttackFailScore() => Score -= _scoreData.AttackFailScore;

        /// <summary>防御失敗時にスコアを減算する</summary>
        public void SubDefenseFailScore() => Score -= _scoreData.DefenseFailScore;
    }
}
