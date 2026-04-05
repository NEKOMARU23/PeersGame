using TechC.Core.Manager;

public class GameManager : Singleton<GameManager>
{
    protected override bool UseDontDestroyOnLoad => true;

    protected override void OnInitialize()
    {
        // ここに初期化処理を書く
    }
}
