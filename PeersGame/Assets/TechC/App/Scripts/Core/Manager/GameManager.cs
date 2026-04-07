using TechC.Core.Manager;

public class GameManager : Singleton<GameManager>
{
    protected override bool UseDontDestroyOnLoad => true;

    protected override void OnInitialize()
    {
        // ここに初期化処理を書く、特にない場合はこのクラス自体が不要になる可能性がある
        // スコアの初期化処理などなにかしら書いてもいい、しかしInGameManagerがスコアの実装を保持している場合は不要
    }
}
