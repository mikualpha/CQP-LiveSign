using cn.mikualpha.livesign.Code;
using Native.Csharp.Sdk.Cqp.EventArgs;
using Native.Csharp.Sdk.Cqp.Interface;
using System;

public class Event_AppStatus : IAppEnable, IAppDisable
{
	#region --公开方法--
	/// <summary>
	/// Type=1003 应用被启用<para/>
	/// 处理 酷Q 的插件启动事件回调
	/// </summary>
	/// <param name="sender">事件的触发对象</param>
	/// <param name="e">事件的附加参数</param>
	public void AppEnable(object sender, CQAppEnableEventArgs e)
	{
        // 当应用被启用后，将收到此事件。
        // 如果酷Q载入时应用已被启用，则在_eventStartup(Type=1001,酷Q启动)被调用后，本函数也将被调用一次。
        // 如非必要，不建议在这里加载窗口。（可以添加菜单，让用户手动打开窗口）
        ApiModel.setModel(e.CQApi, e.CQLog);
        MessageProcessInterface.startCheck();
    }

	/// <summary>
	/// Type=1004 应用被禁用<para/>
	/// 处理 酷Q 的插件关闭事件回调
	/// </summary>
	/// <param name="sender">事件的触发对象</param>
	/// <param name="e">事件的附加参数</param>
	public void AppDisable(object sender, CQAppDisableEventArgs e)
	{
		// 当应用被停用前，将收到此事件。
		// 如果酷Q载入时应用已被停用，则本函数【不会】被调用。
		// 无论本应用是否被启用，酷Q关闭前本函数都【不会】被调用。
        MessageProcessInterface.endCheck();
    }
	#endregion
}
