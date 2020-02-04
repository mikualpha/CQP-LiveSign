class KingkongMessageProcess : MessageProcess
{
    #region --单例模式--
    private static KingkongMessageProcess ins = new KingkongMessageProcess();
    private KingkongMessageProcess() { }

    public static MessageProcess getInstance() { return ins; }

    #endregion

    #region --接口定义--
    protected override string getType()
    {
        return "金刚";
    }

    internal override LiveCheck getCheckInstance()
    {
        return KingkongLiveCheck.getInstance();
    }
    #endregion
}
