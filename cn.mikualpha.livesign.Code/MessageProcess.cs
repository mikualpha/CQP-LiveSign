using Native.Sdk.Cqp.EventArgs;

internal abstract class MessageProcess
{
    protected MessageProcess() { }

    public void processPrivateMsg(CQPrivateMessageEventArgs context)
    {
        string lowerMessage = context.Message.Text.ToLower();
        if (lowerMessage.Contains("/" + getType().ToLower() + "订阅-"))
        {
            int index = context.Message.Text.IndexOf('-');
            string room = stringSecurityProcess(context.Message.Text.Substring(index + 1));
            if (room == "")
            {
                context.FromQQ.SendPrivateMessage("输入格式错误！");
                context.Handler = true;
                return;
            }

            string ownerName = getCheckInstance().getOwnerName(room);
            if (ownerName == "")
            {
                context.FromQQ.SendPrivateMessage("操作异常，可能出现网络错误！");
            }
            else
            {
                getCheckInstance().SubscribeByUser(context.FromQQ.Id, room);
                context.FromQQ.SendPrivateMessage("订阅主播[" + ownerName + "]成功!");
            }
            context.Handler = true;
            return;
        }

        if (lowerMessage.Contains("/" + getType().ToLower() + "取消订阅-"))
        {
            int index = context.Message.Text.IndexOf('-');
            string room = stringSecurityProcess(context.Message.Text.Substring(index + 1));
            if (room == "")
            {
                context.FromQQ.SendPrivateMessage("输入格式错误！");
                context.Handler = true;
                return;
            }
                
            string ownerName = getCheckInstance().getOwnerName(room);
            if (ownerName == "")
            {
                context.FromQQ.SendPrivateMessage("操作异常，可能出现网络错误！");
            }
            else
            {
                getCheckInstance().Desubscribe(context.FromQQ.Id, room);
                context.FromQQ.SendPrivateMessage("取消订阅主播[" + ownerName + "]成功!");
            }
            context.Handler = true;
            return;
        }

        if (lowerMessage.Contains("/" + getType().ToLower() + "订阅查询"))
        {
            context.FromQQ.SendPrivateMessage("您的" + getType() + "订阅列表如下：\r\n" + getCheckInstance().getUserSubscribe(context.FromQQ.Id));
        }

        context.Handler = false;
    }

    public void processGroupMsg(CQGroupMessageEventArgs e)
    {
        //若发送用户及群组不在列表，则直接跳出
        if (!getCheckInstance().isAdmin(e.FromQQ.Id.ToString()) || !getCheckInstance().isGroup(e.FromGroup.Id.ToString()) || e.IsFromAnonymous)
        {
            e.Handler = false;
            return;
        }

        string lowerMessage = e.Message.Text.ToLower();
        if (lowerMessage.Contains("/" + getType().ToLower() + "订阅-"))
        {
            int index = e.Message.Text.IndexOf('-');
            string room = stringSecurityProcess(e.Message.Text.Substring(index + 1));
            if (room == "")
            {
                e.FromGroup.SendGroupMessage("输入格式错误！");
                e.Handler = true;
                return;
            }
                

            string ownerName = getCheckInstance().getOwnerName(room);
            if (ownerName == "")
            {
                e.FromGroup.SendGroupMessage("操作异常，可能出现网络错误！");
            }
            else
            {
                getCheckInstance().SubscribeByGroup(e.FromGroup.Id, room);
                e.FromGroup.SendGroupMessage("订阅主播[" + ownerName + "]成功!");
            }
            e.Handler = true;
            return;
        }

        if (lowerMessage.Contains("/" + getType().ToLower() + "取消订阅-"))
        {
            int index = e.Message.Text.IndexOf('-');
            string room = stringSecurityProcess(e.Message.Text.Substring(index + 1));
            if (room == "")
            {
                e.FromGroup.SendGroupMessage("输入格式错误！");
                e.Handler = true;
                return;
            }

            string ownerName = getCheckInstance().getOwnerName(room);
            if (ownerName == "")
            {
                e.FromGroup.SendGroupMessage("操作异常，可能出现网络错误！");
            }
            else
            {
                getCheckInstance().Desubscribe(e.FromGroup.Id, room, 1);
                e.FromGroup.SendGroupMessage("取消订阅主播[" + ownerName + "]成功!");
            }
            e.Handler = true;
            return;
        }

        if (lowerMessage.Contains("/" + getType().ToLower() + "订阅查询"))
        {
            e.FromGroup.SendGroupMessage("本群" + getType() + "订阅列表如下：\r\n" + getCheckInstance().getUserSubscribe(e.FromGroup.Id));
        }

        e.Handler = false;
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
