using Newtonsoft.Json;
using Native.Tool.Http;
using System.Text;
using System;
using System.Collections.Generic;

class KingkongLiveCheck : LiveCheck
{
    #region --单例模式--
    private static KingkongLiveCheck ins = new KingkongLiveCheck();
    private KingkongLiveCheck() { }

    public static LiveCheck getInstance() { return ins; }
    #endregion

    #region --接口实现--
    protected override SQLiteManager getSQLiteManager()
    {
        return KingkongSQLiteManager.getInstance();
    }

    protected override string getHttp(string room)
    {
        try
        {
            return Encoding.UTF8.GetString(HttpWebClient.Get("https://api-kk.lv-play.com/webapi/v1/search/global?keyword=" + room));
        }
        catch (Exception)
        {
            return "";
        }

    }

    public override string getOwnerName(string room)
    {
        KingkongData data = getJson(room);
        if (data == null) return "";
        foreach (KingkongUser user in data.users)
        {
            if (user.room_id == room) return user.nickname;
        }
        return "";
    }

    private KingkongUser userTemp = null;
    protected override int getDataRoomStatus(string room)
    {
        KingkongData dataTemp = getJson(room);
        if (dataTemp == null) return (int)LivingStatus.ERROR;
        foreach (KingkongUser user in dataTemp.users)
        {
            if (user.room_id == room)
            {
                userTemp = user;
                return user.live_status;
            }
        }
        return (int)LivingStatus.ERROR;
    }

    protected override string getOnlineMessageModel()
    {
        string msg = "主播[" + userTemp.nickname + "]开播啦！" +
            getEasterEggStr(userTemp.room_id) +
            "\n直播间地址：https://play.lang.live/" + userTemp.room_id;
        return msg;
    }

    protected override string getEasterEggStr(string id)
    {
        string output = "";
        if (getOptions()["EasterEgg"] == "0") return output;

        output += (id == "3116963" ? "（爽粉们米缸开啦！）" : "");
        return output;
    }

    private KingkongData getJson(string room)
    {
        try
        {
            string json = getHttp(room);
            if (json == null || json == "") return null;
            if (!json.Contains("{")) return null;
            KingkongDataTemp result = JsonConvert.DeserializeObject<KingkongDataTemp>(json);
            if (result == null) return null;
            return result.data;
        }
        catch (Exception)
        {
            return null;
        }
    }

    protected class KingkongDataTemp
    {
        public KingkongData data { get; set; }
    }

    protected class KingkongData
    {
        public List<KingkongUser> users { get; set; }
    }

    protected class KingkongUser
    {
        public string room_id { get; set; }
        public string nickname { get; set; }
        public int live_status { get; set; }
    }
    #endregion
}
