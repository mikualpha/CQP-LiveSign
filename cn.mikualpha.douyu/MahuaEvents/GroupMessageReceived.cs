using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;

namespace cn.mikualpha.douyu.MahuaEvents
{
    /// <summary>
    /// 群消息接收事件
    /// </summary>
    public class GroupMessageReceived
        : IGroupMessageReceivedMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public GroupMessageReceived(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void ProcessGroupMessage(GroupMessageReceivedContext context)
        {
            //若发送用户及群组不在列表，则直接跳出
            if (!DouyuCheck.getInstance().isAdmin(context.FromQq) || !DouyuCheck.getInstance().isGroup(context.FromGroup)) return;

            if (context.Message.Contains("/斗鱼订阅-"))
            {
                int index = context.Message.IndexOf('-');
                string room = context.Message.Substring(index + 1);
                DouyuCheck.getInstance().SubscribeByGroup(context.FromGroup, room);
                _mahuaApi.SendGroupMessage(context.FromGroup).Text("订阅主播[").Text(DouyuCheck.getInstance().getOwner(room)).Text("]成功!").Done();
            }

            if (context.Message.Contains("/斗鱼取消订阅-"))
            {
                int index = context.Message.IndexOf('-');
                string room = context.Message.Substring(index + 1);
                DouyuCheck.getInstance().Desubscribe(context.FromGroup, room, 1);
                _mahuaApi.SendGroupMessage(context.FromGroup).Text("取消订阅主播[").Text(DouyuCheck.getInstance().getOwner(room)).Text("]成功!").Done();
            }
        }
    }
}
