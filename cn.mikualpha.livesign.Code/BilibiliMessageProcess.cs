using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BilibiliMessageProcess : MessageProcess
{
    #region --单例模式--
    private static BilibiliMessageProcess ins = new BilibiliMessageProcess();
    private BilibiliMessageProcess() { }

    public static MessageProcess getInstance() { return ins; }

    #endregion

    #region --接口定义--
    protected override string getType()
    {
        return "B站";
    }

    internal override LiveCheck getCheckInstance()
    {
        return BilibiliLiveCheck.getInstance();
    }
    #endregion
}
