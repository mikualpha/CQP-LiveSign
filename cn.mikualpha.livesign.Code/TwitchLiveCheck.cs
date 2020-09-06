using Newtonsoft.Json;
using Native.Tool.Http;
using System.Text;
using System.Net;
using System;
using System.Collections.Generic;

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
        string userid = getUserID(room);
        return getHttpProxy("https://api.twitch.tv/kraken/channels/" + userid, getHeaders());
    }

    private string getStreamInfo(string room)
    {
        string userid = getUserID(room);
        return getHttpProxy("https://api.twitch.tv/kraken/streams/" + userid, getHeaders());
    }

    private string getUserID(string username)
    {
        try
        {
            string userJson = getHttpProxy("https://api.twitch.tv/kraken/users?login=" + username, getHeaders());
            TwitchUsers users = JsonConvert.DeserializeObject<TwitchUsers>(userJson);
            return users.users[0]._id;
        } catch (Exception) {
            return "";
        }
    }

    private Dictionary<string, string> getHeaders()
    {
        Dictionary<string, string> output = new Dictionary<string, string>();
        output.Add("Accept", "application/vnd.twitchtv.v5+json");
        output.Add("Client-ID", client_id);
        return output;
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
            getEasterEggStr(dataTemp.display_name) +
            "\n直播间地址：" + dataTemp.url;
        return msg;
    }

    protected override string getEasterEggStr(string id)
    {
        string output = "";
        if (getOptions()["EasterEgg"] == "0") return output;

        output += (id == "wuyikoei" ? "（爽粉们米缸开啦！）" : "");
        return output;
    }

    private TwitchChannel getChannelJson(string room)
    {
        try
        {
            string json = getHttp(room);
            if (json == null || json == "") return null;
            if (!json.Contains("{")) return null;
            if (json.Contains("\"error\"")) return null;

            TwitchChannel result = JsonConvert.DeserializeObject<TwitchChannel>(json);
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

    protected class TwitchUsers
    {
        public List<TwitchUser> users { get; set; }
        public class TwitchUser
        {
            public string _id { get; set; }
        }
    }

    #endregion
}