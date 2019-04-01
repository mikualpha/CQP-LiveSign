using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Native.Csharp.App.Event
{
    class DouyuMessageProcess : MessageProcess
    {
        #region --单例模式--
        private static DouyuMessageProcess ins = new DouyuMessageProcess();
        private DouyuMessageProcess() { }

        public static MessageProcess getInstance() { return ins; }

        #endregion

        #region --接口定义--
        protected override string getType()
        {
            return "斗鱼";
        }

        internal override LiveCheck getCheckInstance()
        {
            return DouyuLiveCheck.getInstance();
        }
        #endregion
    }
}
