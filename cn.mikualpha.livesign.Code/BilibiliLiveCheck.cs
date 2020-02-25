using Newtonsoft.Json;
using Native.Tool.Http;
using System.Text;
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
            return Encoding.UTF8.GetString(HttpWebClient.Get("https://api.live.bilibili.com/xlive/web-room/v1/index/getInfoByRoom?room_id=" + room));
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
        return data.anchor_info.base_info.uname;
    }

    private BilibiliData dataTemp = null;
    protected override int getDataRoomStatus(string room)
    {
        dataTemp = getJson(room);
        if (dataTemp == null) return (int)LivingStatus.ERROR;
        return (int)(dataTemp.room_info.live_status == 1 ? LivingStatus.ONLINE : LivingStatus.OFFLINE);
    }

    protected override string getOnlineMessageModel()
    {
        string msg = "主播[" + dataTemp.anchor_info.base_info.uname + "]开播啦！" +
            getEasterEggStr(dataTemp.room_info.room_id.ToString()) +
            "\n直播间地址：https://live.bilibili.com/" + dataTemp.room_info.room_id.ToString();
        return msg;
    }

    protected override string getEasterEggStr(string id)
    {
        return "";
    }

    private BilibiliData getJson(string room)
    {
        try
        {
            string json = getHttp(room);
            if (json == null || json == "") return null;
            if (!json.Contains("{")) return null;
            BilibiliDataTemp result = JsonConvert.DeserializeObject<BilibiliDataTemp>(json);
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
        public BilibiliRoomInfo room_info { get; set; }
        public BilibiliAnchorInfo anchor_info { get; set; }

        public class BilibiliRoomInfo
        {
            public int room_id { get; set; }
            public int live_status { get; set; }
            public string title { get; set; }
        }

        public class BilibiliAnchorInfo
        {
            public BilibiliBaseInfo base_info { get; set; }
            public class BilibiliBaseInfo
            {
                public string uname { get; set; }
            }
        }

    }
    #endregion
}