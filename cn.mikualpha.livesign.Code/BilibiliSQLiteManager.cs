using cn.mikualpha.livesign.Code;

class BilibiliSQLiteManager : SQLiteManager
{
    private static BilibiliSQLiteManager ins = new BilibiliSQLiteManager();
    private BilibiliSQLiteManager() { }
    public static SQLiteManager getInstance() { return ins; }

    private readonly string path = ApiModel.CQApi.AppDirectory + "BilibiliConfig.db";
    public override string getDatabasePath()
    {
        return path;
    }
}