using System.Collections.Generic;

namespace Native.Csharp.App.Event
{
    class MessageProcessInterface
    {
        //TO-DO：添加新接口时更改此项
        private static List<MessageProcess> list = new List<MessageProcess> { DouyuMessageProcess.getInstance(), BilibiliMessageProcess.getInstance(), KingkongMessageProcess.getInstance(), TwitchMessageProcess.getInstance() };
        //调用接口汇总，方便统一修改
        public static void processPrivateMessage(Model.PrivateMessageEventArgs context)
        {
            foreach (MessageProcess messageObserver in list)
            {
                messageObserver.processPrivateMsg(context);
                if (context.Handled) return;
            }
        }

        public static void processGroupMessage(Model.GroupMessageEventArgs context)
        {
            foreach (MessageProcess messageObserver in list)
            {
                messageObserver.processGroupMsg(context);
                if (context.Handled) return;
            }
        }

        public static void startCheck()
        {
            foreach (MessageProcess messageObserver in list)
            {
                messageObserver.getCheckInstance().startCheck();
            }
        }

        public static void endCheck()
        {
            foreach (MessageProcess messageObserver in list)
            {
                messageObserver.getCheckInstance().endCheck();
            }
        }
    }
}
