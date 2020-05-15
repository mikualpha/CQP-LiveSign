class TwitchSQLiteManager : SQLiteManager
{
    private static TwitchSQLiteManager ins = new TwitchSQLiteManager();
    private TwitchSQLiteManager() { }
    public static SQLiteManager getInstance() { return ins; }

    private readonly string path = ApiModel.CQApi.AppDirectory + "TwitchConfig.db";
    public override string getDatabasePath()
    {
        return path;
    }
}
