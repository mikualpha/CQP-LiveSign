using Newtonsoft.Json;
using Native.Csharp.Tool.Http;
using System.Text;
using System.Net;
using System;

class BilibiliLiveCheck : LiveCheck
{
    #region --单例模式--
    private static BilibiliLiveCheck ins = new BilibiliLiveCheck();
    private BilibiliLiveCheck() { }

    public static LiveCheck getInstance() { return ins; }
    #endregion

    #region --接口实现--
    protected override SQLiteManager getSQLiteManager()
    {
        return BilibiliSQLiteManager.getInstance();
    }

    protected override string getHttp(string room)
    {
        try
        {
            return Encoding.UTF8.GetString(HttpWebClient.Get("http://api.live.bilibili.com/AppRoom/index?platform=android&room_id=" + room));
        }
        catch (Exception)
        {
            return "";
        }

    }

    public override string getOwnerName(string room)
    {
        BilibiliData data = getJson(room);
        if (data == null) return "";
        return data.uname;
    }

    private BilibiliData dataTemp = null;
    protected override int getDataRoomStatus(string room)
    {
        dataTemp = getJson(room);
        if (dataTemp == null) return (int)LivingStatus.ERROR;
        return (int)(dataTemp.status == "LIVE" ? LivingStatus.ONLINE : LivingStatus.OFFLINE);
    }

    protected override string getOnlineMessageModel()
    {
        string msg = "主播[" + dataTemp.uname + "]开播啦！" +
            "\n直播间地址：https://live.bilibili.com/" + dataTemp.room_id.ToString();
        return msg;
    }

    private BilibiliData getJson(string room)
    {
        try
        {
            string json = getHttp(room);
            if (json == null || json == "") return null;
            if (!json.Contains("{")) return null;
            BilibiliDataTemp result = (BilibiliDataTemp)JsonConvert.DeserializeObject(json, typeof(BilibiliDataTemp));
            if (result == null) return null;
            return result.data;
        } catch (Exception) {
            return null;
        }
    }

    protected class BilibiliDataTemp
    {
        public BilibiliData data { get; set; }
    }

    protected class BilibiliData
    {
        public int room_id { get; set; }
        public string status { get; set; }
        public string title { get; set; }
        public string uname { get; set; }
    }
    #endregion
}