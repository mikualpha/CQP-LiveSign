using Native.Csharp.App;

class BilibiliSQLiteManager : SQLiteManager
{
    private static BilibiliSQLiteManager ins = new BilibiliSQLiteManager();
    private BilibiliSQLiteManager() { }
    public static SQLiteManager getInstance() { return ins; }

    private readonly string path = Common.CqApi.GetAppDirectory() + "BilibiliConfig.db";
    public override string getDatabasePath()
    {
        return path;
    }
}