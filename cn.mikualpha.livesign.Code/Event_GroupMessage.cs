using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;

public class Event_GroupMessage : IGroupMessage
{
	#region --公开方法--
	/// <summary>
	/// Type=2 群消息<para/>
	/// 处理收到的群消息
	/// </summary>
	/// <param name="sender">事件的触发对象</param>
	/// <param name="e">事件的附加参数</param>
	public void GroupMessage(object sender, CQGroupMessageEventArgs e)
	{
		// 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
		// 这里处理消息
		if (e.IsFromAnonymous)
		{
			e.Handler = false;
			return;
		}
        MessageProcessInterface.processGroupMessage(e);
	}
	#endregion
}
