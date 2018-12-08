using Native.Csharp.Sdk.Cqp.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Native.Csharp.App.Model;

namespace Native.Csharp.App.Event
{
	public class Event_PrivateMessage
	{
		#region --字段--
		private static Event_PrivateMessage _instance = new Event_PrivateMessage();
		#endregion

		#region --属性--
		/// <summary>
		/// 获取 Event_ReceiveMessage 实例对象
		/// </summary>
		public static Event_PrivateMessage Instance { get => _instance; }
		#endregion

		#region --构造函数--
		/// <summary>
		/// 隐藏构造函数
		/// </summary>
		private Event_PrivateMessage()
		{

		}
		#endregion

		#region --公开方法--
		/// <summary>
		/// Type=21 好友消息
		/// </summary>
		/// <param name="sender">触发此事件的对象</param>
		/// <param name="e">附加的参数</param>
		public void ReceiveFriendMessage(object sender, PrivateMessageEventArgs e)
		{
            //本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            //这里处理消息

            processMsg(e);
        }

		/// <summary>
		/// Type=21 在线状态消息
		/// </summary>
		/// <param name="sender">触发此事件的对象</param>
		/// <param name="e">附加的参数</param>
		public void ReceiveOnlineStatusMessage(object sender, PrivateMessageEventArgs e)
		{
			//本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
			//这里处理消息

			e.Handled = false;  //关于返回说明, 请参见 "Event_ReceiveMessage.ReceiveFriendMessage" 方法
		}

		/// <summary>
		/// Type=21 群私聊消息
		/// </summary>
		/// <param name="sender">触发此事件的对象</param>
		/// <param name="e">附加的参数</param>
		public void ReceiveGroupPrivateMessage(object sender, PrivateMessageEventArgs e)
		{
            //本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            //这里处理消息
            processMsg(e);
		}

		/// <summary>
		/// Type=21 讨论组私聊消息
		/// </summary>
		/// <param name="sender">触发此事件的对象</param>
		/// <param name="e">附加的参数</param>
		public void ReceiveDiscussPrivateMessage(object sender, PrivateMessageEventArgs e)
		{
            //本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
            //这里处理消息
            processMsg(e);
        }
        #endregion

        #region --自用函数--
        private void processMsg(PrivateMessageEventArgs context)
        {
            if (context.Msg.Contains("/斗鱼订阅-"))
            {
                int index = context.Msg.IndexOf('-');
                string room = context.Msg.Substring(index + 1);
                DouyuCheck.getInstance().SubscribeByUser(context.FromQQ.ToString(), room);
                EnApi.Instance.SendPrivateMessage(context.FromQQ, "订阅主播[" + DouyuCheck.getInstance().getOwner(room) + "]成功!");
                context.Handled = true;
                return;
            }

            if (context.Msg.Contains("/斗鱼取消订阅-"))
            {
                int index = context.Msg.IndexOf('-');
                string room = context.Msg.Substring(index + 1);
                DouyuCheck.getInstance().Desubscribe(context.FromQQ.ToString(), room);
                EnApi.Instance.SendPrivateMessage(context.FromQQ, "取消订阅主播[" + DouyuCheck.getInstance().getOwner(room) + "]成功!");
                context.Handled = true;
                return;
            }

            context.Handled = false;
        }
        #endregion
    }
}
