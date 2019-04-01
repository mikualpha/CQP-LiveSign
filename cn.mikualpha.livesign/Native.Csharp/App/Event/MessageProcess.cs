namespace Native.Csharp.App.Event
{
    internal abstract class MessageProcess
    {
        protected MessageProcess() { }

        public void processPrivateMsg(Model.PrivateMessageEventArgs context)
        {
            if (context.Msg.Contains("/" + getType() + "订阅-"))
            {
                int index = context.Msg.IndexOf('-');
                string room = stringSecurityProcess(context.Msg.Substring(index + 1));
                if (room == "")
                {
                    Common.CqApi.SendGroupMessage(context.FromQQ, "输入格式错误！");
                    context.Handled = true;
                    return;
                }
                getCheckInstance().SubscribeByUser(context.FromQQ, room);

                string ownerName = getCheckInstance().getOwnerName(room);
                if (ownerName == "") Common.CqApi.SendPrivateMessage(context.FromQQ, "操作异常，可能出现网络错误！");
                else Common.CqApi.SendPrivateMessage(context.FromQQ, "订阅主播[" + ownerName + "]成功!");
                context.Handled = true;
                return;
            }

            if (context.Msg.Contains("/" + getType() + "取消订阅-"))
            {
                int index = context.Msg.IndexOf('-');
                string room = stringSecurityProcess(context.Msg.Substring(index + 1));
                if (room == "")
                {
                    Common.CqApi.SendGroupMessage(context.FromQQ, "输入格式错误！");
                    context.Handled = true;
                    return;
                }
                getCheckInstance().Desubscribe(context.FromQQ, room);

                string ownerName = getCheckInstance().getOwnerName(room);
                if (ownerName == "") Common.CqApi.SendPrivateMessage(context.FromQQ, "操作异常，可能出现网络错误！");
                else Common.CqApi.SendPrivateMessage(context.FromQQ, "取消订阅主播[" + ownerName + "]成功!");
                context.Handled = true;
                return;
            }

            if (context.Msg.Contains("/" + getType() + "订阅查询"))
            {
                Common.CqApi.SendPrivateMessage(context.FromQQ, "您的" + getType() + "订阅列表如下：\r\n" + getCheckInstance().getUserSubscribe(context.FromQQ));
            }

            context.Handled = false;
        }

        public void processGroupMsg(Model.GroupMessageEventArgs e)
        {
            //若发送用户及群组不在列表，则直接跳出
            if (!getCheckInstance().isAdmin(e.FromQQ.ToString()) || !getCheckInstance().isGroup(e.FromGroup.ToString()))
            {
                e.Handled = false;
                return;
            }

            if (e.Msg.Contains("/" + getType() + "订阅-"))
            {
                int index = e.Msg.IndexOf('-');
                string room = stringSecurityProcess(e.Msg.Substring(index + 1));
                if (room == "")
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "输入格式错误！");
                    e.Handled = true;
                    return;
                }
                getCheckInstance().SubscribeByGroup(e.FromGroup, room);

                string ownerName = getCheckInstance().getOwnerName(room);
                if (ownerName == "") Common.CqApi.SendGroupMessage(e.FromGroup, "操作异常，可能出现网络错误！");
                else Common.CqApi.SendGroupMessage(e.FromGroup, "订阅主播[" + ownerName + "]成功!");
                e.Handled = true;
                return;
            }

            if (e.Msg.Contains("/" + getType() + "取消订阅-"))
            {
                int index = e.Msg.IndexOf('-');
                string room = stringSecurityProcess(e.Msg.Substring(index + 1));
                if (room == "")
                {
                    Common.CqApi.SendGroupMessage(e.FromGroup, "输入格式错误！");
                    e.Handled = true;
                    return;
                }
                getCheckInstance().Desubscribe(e.FromGroup, room, 1);

                string ownerName = getCheckInstance().getOwnerName(room);
                if (ownerName == "") Common.CqApi.SendGroupMessage(e.FromGroup, "操作异常，可能出现网络错误！");
                else Common.CqApi.SendGroupMessage(e.FromGroup, "取消订阅主播[" + ownerName + "]成功!");
                e.Handled = true;
                return;
            }

            if (e.Msg.Contains("/" + getType() + "订阅查询"))
            {
                Common.CqApi.SendGroupMessage(e.FromGroup, "本群" + getType() + "订阅列表如下：\r\n" + getCheckInstance().getUserSubscribe(e.FromGroup));
            }

            e.Handled = false;
        }

        //对纯数字输入进行处理，以免出现异常，可通过重写删除
        protected virtual string stringSecurityProcess(string input)
        {
            try { return long.Parse(input).ToString(); }
            catch (System.FormatException) { return ""; }
        }

        protected abstract string getType();

        internal abstract LiveCheck getCheckInstance();
    }
}
