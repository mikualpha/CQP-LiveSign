using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;

namespace cn.mikualpha.douyu.MahuaEvents
{
    /// <summary>
    /// 私聊消息接收事件
    /// </summary>
    public class PrivateMessageReceived
        : IPrivateMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PrivateMessageReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessPrivateMessage(PrivateMessageReceivedContext context)
        {
            if (context.Message.Contains("/斗鱼订阅-"))
            {
                int index = context.Message.IndexOf('-');
                string room = context.Message.Substring(index + 1);
                DouyuCheck.getInstance().SubscribeByUser(context.FromQq, room);
                _mahuaApi.SendPrivateMessage(context.FromQq).Text("订阅主播[").Text(DouyuCheck.getInstance().getOwner(room)).Text("]成功!").Done();
            }

            if (context.Message.Contains("/斗鱼取消订阅-"))
            {
                int index = context.Message.IndexOf('-');
                string room = context.Message.Substring(index + 1);
                DouyuCheck.getInstance().Desubscribe(context.FromQq, room);
                _mahuaApi.SendPrivateMessage(context.FromQq).Text("取消订阅主播[").Text(DouyuCheck.getInstance().getOwner(room)).Text("]成功!").Done();
            }
        }
    }
}
