using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;
using System.Threading;

namespace cn.mikualpha.douyu.MahuaEvents
{
    /// <summary>
    /// 插件被启用事件
    /// </summary>
    public class PluginEnabled
        : IPluginEnabledMahuaEvent
    {
        public static IMahuaApi _mahuaApi;

        public PluginEnabled(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Enabled(PluginEnabledContext context)
        {
            DouyuCheck.getInstance().startCheck();
        }

    }
}
