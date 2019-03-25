using Native.Csharp.App;

class DouyuSQLiteManager : SQLiteManager
{
    private static DouyuSQLiteManager ins = new DouyuSQLiteManager();
    private DouyuSQLiteManager() { }
    public static SQLiteManager getInstance() { return ins; }

    private readonly string path = Common.CqApi.GetAppDirectory() + "DouyuConfig.db";
    public override string getDatabasePath()
    {
        return path;
    }
}
