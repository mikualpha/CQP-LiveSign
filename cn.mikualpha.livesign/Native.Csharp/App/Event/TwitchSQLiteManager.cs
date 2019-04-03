using Native.Csharp.App;

class TwitchSQLiteManager : SQLiteManager
{
    private static TwitchSQLiteManager ins = new TwitchSQLiteManager();
    private TwitchSQLiteManager() { }
    public static SQLiteManager getInstance() { return ins; }

    private readonly string path = Common.CqApi.GetAppDirectory() + "TwitchConfig.db";
    public override string getDatabasePath()
    {
        return path;
    }
}
