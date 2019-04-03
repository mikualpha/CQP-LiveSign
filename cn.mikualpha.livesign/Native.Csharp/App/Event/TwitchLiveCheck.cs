using Newtonsoft.Json;
using Native.Csharp.Tool.Http;
using System.Text;
using System.Net;
using System;

class TwitchLiveCheck : LiveCheck
{
    #region --单例模式--
    private static TwitchLiveCheck ins = new TwitchLiveCheck();
    private TwitchLiveCheck() { }

    public static LiveCheck getInstance() { return ins; }
    #endregion

    #region --接口实现--
    private const string client_id = "356pgjwnm9953cgwrln8d1vzsyvmlv";

    protected override SQLiteManager getSQLiteManager()
    {
        return TwitchSQLiteManager.getInstance();
    }

    protected override string getHttp(string room) //频道信息获取
    {
        return getHttpProxy("https://api.twitch.tv/kraken/channels/" + room + "?client_id=" + client_id);
    }

    private string getStreamInfo(string room)
    {
        return getHttpProxy("https://api.twitch.tv/kraken/streams/" + room + "?client_id=" + client_id);
    }

    public override string getOwnerName(string room)
    {
        TwitchChannel data = getChannelJson(room);
        if (data == null) return "";
        return data.display_name;
    }

    private TwitchChannel dataTemp = null;
    protected override int getDataRoomStatus(string room)
    {
        string json = getStreamInfo(room);
        if (json == "" || json == null || !json.Contains("{")) return (int)LivingStatus.ERROR;
        if (json.Contains("\"error\"")) return (int)LivingStatus.ERROR;
        if (json.Contains("\"stream_type\":\"live\""))
        {
            dataTemp = getChannelJson(room);
            return (int)LivingStatus.ONLINE;
        }
        return (int)LivingStatus.OFFLINE;
    }

    protected override string getOnlineMessageModel()
    {
        string msg = "主播[" + dataTemp.display_name + "]开播啦！" +
            (dataTemp.display_name == "wuyikoei" ? "（爽粉们米缸开啦！）" : "") +
            "\n直播间地址：" + dataTemp.url;
        return msg;
    }

    private TwitchChannel getChannelJson(string room)
    {
        try
        {
            string json = getHttp(room);
            if (json == null || json == "") return null;
            if (!json.Contains("{")) return null;
            if (json.Contains("\"error\"")) return null;

            TwitchChannel result = (TwitchChannel)JsonConvert.DeserializeObject(json, typeof(TwitchChannel));
            if (result == null) return null;
            return result;
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected class TwitchChannel
    {
        public string display_name { get; set; }
        public string status { get; set; }
        public string game { get; set; }
        public string url { get; set; }
    }

    #endregion
}