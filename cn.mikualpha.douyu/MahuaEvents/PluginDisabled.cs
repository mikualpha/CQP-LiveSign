using Newbe.Mahua;
using Newbe.Mahua.MahuaEvents;
using System;

namespace cn.mikualpha.douyu.MahuaEvents
{
    /// <summary>
    /// 插件被禁用事件
    /// </summary>
    public class PluginDisabled
        : IPluginDisabledMahuaEvent
    {
        private readonly IMahuaApi _mahuaApi;

        public PluginDisabled(
            IMahuaApi mahuaApi)
        {
            _mahuaApi = mahuaApi;
        }

        public void Disable(PluginDisabledContext context)
        {
            DouyuCheck.getInstance().endCheck();
        }
    }
}
