using System.Collections.Generic;

namespace Native.Csharp.App.Event
{
    class MessageProcessInterface
    {
        //TO-DO：添加新接口时更改此项
        private static List<MessageProcess> list = new List<MessageProcess>{ DouyuMessageProcess.getInstance(), BilibiliMessageProcess.getInstance() };
        //调用接口汇总，方便统一修改
        public static void processPrivateMessage(Model.PrivateMessageEventArgs context)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].processPrivateMsg(context);
                if (context.Handled) return;
            }
        }

        public static void processGroupMessage(Model.GroupMessageEventArgs context)
        {
            for (int i = 0; i < list.Count; ++i)
            {
                list[i].processGroupMsg(context);
                if (context.Handled) return;
            }
        }
    }
}
