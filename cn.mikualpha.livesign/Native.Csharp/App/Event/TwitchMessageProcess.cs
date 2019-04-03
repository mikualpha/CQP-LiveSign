using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Native.Csharp.App.Event
{
    class TwitchMessageProcess : MessageProcess
    {
        #region --单例模式--
        private static TwitchMessageProcess ins = new TwitchMessageProcess();
        private TwitchMessageProcess() { }

        public static MessageProcess getInstance() { return ins; }

        #endregion

        #region --接口定义--
        protected override string getType()
        {
            return "Twitch";
        }

        internal override LiveCheck getCheckInstance()
        {
            return TwitchLiveCheck.getInstance();
        }

        protected override string stringSecurityProcess(string input)
        {
            Regex rgx = new Regex("/[^\\w\\-]/g");
            input = rgx.Replace(input, "");
            return input;
        }
        #endregion
    }
}
