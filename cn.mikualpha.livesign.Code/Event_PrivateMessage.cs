using Native.Sdk.Cqp.EventArgs;
using Native.Sdk.Cqp.Interface;

public class Event_PrivateMessage : IPrivateMessage
{
	#region --公开方法--
	public void PrivateMessage(object sender, CQPrivateMessageEventArgs e)
	{
        // 本子程序会在酷Q【线程】中被调用，请注意使用对象等需要初始化(CoInitialize,CoUninitialize)。
        // 这里处理消息
        MessageProcessInterface.processPrivateMessage(e);
    }
	#endregion
}
