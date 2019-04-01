using Native.Csharp.App;

class KingkongSQLiteManager : SQLiteManager
{
    private static KingkongSQLiteManager ins = new KingkongSQLiteManager();
    private KingkongSQLiteManager() { }
    public static SQLiteManager getInstance() { return ins; }

    private readonly string path = Common.CqApi.GetAppDirectory() + "KingkongConfig.db";
    public override string getDatabasePath()
    {
        return path;
    }
}
