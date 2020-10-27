using Newtonsoft.Json;
using Native.Tool.Http;
using System.Text;
using System.Net;
using System;

class DouyuLiveCheck : LiveCheck
{
    #region --单例模式--
    private static DouyuLiveCheck ins = new DouyuLiveCheck();
    private DouyuLiveCheck() { }

    public static LiveCheck getInstance() { return ins; }
    #endregion

    #region --接口实现--
    protected override SQLiteManager getSQLiteManager()
    {
        return DouyuSQLiteManager.getInstance();
    }

    protected override string getHttp(string room)
    {
        try {
            //WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
            //CookieCollection cookies = new CookieCollection();
            string content = Encoding.UTF8.GetString(HttpWebClient.Get("http://open.douyucdn.cn/api/RoomApi/room/" + room)); //, "", ref cookies, ref webHeaderCollection, new WebProxy("127.0.0.1", 8888), Encoding.UTF8));
            return content;
        } catch (Exception) {
            return "";
        }
    }

    public override string getOwnerName(string room)
    {
        DouyuData data = getJson(room);
        if (data == null) return "";
        return data.owner_name;
    }

    private DouyuData dataTemp = null;
    protected override int getDataRoomStatus(string room)
    {
        dataTemp = getJson(room);
        if (dataTemp == null) return (int)LivingStatus.ERROR;
        return int.Parse(dataTemp.room_status);
    }

    protected override string getOnlineMessageModel()
    {
        string msg = "主播[" + dataTemp.owner_name + "]开播啦！" +
            getEasterEggStr(dataTemp.room_id.ToString()) + 
            "\n直播间地址：https://www.douyu.com/" + dataTemp.room_id.ToString();
        return msg;
    }

    private DouyuData getJson(string room)
    {
        try
        {
            string json = getHttp(room);
            if (json == null || json == "") return null;
            if (!json.Contains("{")) return null;
            DouyuDataTemp result = JsonConvert.DeserializeObject<DouyuDataTemp>(json);
            if (result == null) return null;
            return result.data;
        } catch (Exception) {
            return null;
        }
    }

    protected class DouyuDataTemp
    {
        public DouyuData data { get; set; }
    }

    protected class DouyuData
    {
        public int room_id { get; set; }
        public string room_status { get; set; }
        public string owner_name { get; set; }
    }
    #endregion
}