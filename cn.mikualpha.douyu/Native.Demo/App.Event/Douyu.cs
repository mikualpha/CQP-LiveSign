using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Threading;
using Native.Sdk.Cqp.Api;
using Native.Sdk.Cqp.Tool;
using System.Net.Http;
using System.Threading.Tasks;

class DouyuCheck
{
    private static DouyuCheck ins = null;
    private Thread thread = null;
    private string dir = EnApi.Instance.GetAppDirectory();
    private string path = EnApi.Instance.GetAppDirectory() + "config.ini";
    public bool running = false;
    private DouyuCheck() { initalizeFile(); }

    public static DouyuCheck getInstance()
    {
        if (ins == null) ins = new DouyuCheck();
        return ins;
    }

    private bool initalizeFile()
    {
        if (File.Exists(path)) return false;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        StreamWriter writer = new StreamWriter(fs);
        writer.Write("Group=0\r\n" +
                    "Admin=0\r\n");
        writer.Close();
        fs.Close();
        return true;
    }

    public bool isGroup(string input)
    {
        string[] temp = (readFromFile()[0] as string).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in temp)
        {
            if (i == "0") return true;
            if (i == input) return true;
        }
        return false;
    }

    public bool isAdmin(string input)
    {
        string[] temp = (readFromFile()[1] as string).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in temp)
        {
            if (i == "0") return true;
            if (i == input) return true;
        }
        return false;
    }

    public void startCheck()
    {
        running = true;
        thread = new Thread(checkStatus);
        thread.Start();
    }

    public void endCheck()
    {
        try
        {
            running = false;
            thread.Abort();
        }
        catch (ThreadAbortException) { }
    }

    private void checkStatus()
    {
        while (running)
        {
            string[] rooms = SQLiteManager.getInstance().getRooms();
            foreach (string i in rooms)
            {
                DouyuData status = getJson(i);
                if (status == null) break;

                if (SQLiteManager.getInstance().getLiveStatus(i) != status.room_status)
                {
                    SQLiteManager.getInstance().setLiveStatus(i, status.room_status);
                    if (status.room_status == 1) //正在直播
                    {
                        string[] users = SQLiteManager.getInstance().getUserByRoom(i); //获取所有订阅用户并发送消息
                        foreach (string j in users) { sendPrivateMessage(status, j); }

                        string[] groups = SQLiteManager.getInstance().getGroupByRoom(i); //获取所有订阅群组并发送消息
                        foreach (string k in groups) { sendGroupMessage(status, k); }
                    }
                }
            }
            Thread.Sleep(3000);
        }
    }

    private string getOnlineMessage(DouyuData data) //获取发送消息格式
    {
        string msg = "主播[" + data.owner_name + "]开播啦！" +
            (data.room_id == 6655 ? "（爽粉们米缸开啦！）" : "") +
            (data.room_id == 3484 ? "孙一峰永远是我大哥！" : "") +
            "\n直播间地址：https://www.douyu.com/" + data.room_id.ToString();
        return msg;
    }

    private void sendGroupMessage(DouyuData data, string group)
    {
        EnApi.Instance.SendGroupMessage(long.Parse(group), getOnlineMessage(data));
    }

    private void sendPrivateMessage(DouyuData data, string qq)
    {
        EnApi.Instance.SendPrivateMessage(long.Parse(qq), getOnlineMessage(data));
    }

    public string getOwner(string room)
    {
        DouyuData temp = getJson(room);
        if (temp == null) return "";
        return temp.owner_name;
    }

    public int getStatus(string room)
    {
        DouyuData temp = getJson(room);
        if (temp == null) return 2;
        return temp.room_status;
    }

    private DouyuData getJson(string room)
    {
        string json = getHttp(room).GetAwaiter().GetResult();
        if (json == null) return null;
        DouyuDataTemp result = (DouyuDataTemp)JsonConvert.DeserializeObject(json, typeof(DouyuDataTemp));
        if (result == null) return null;
        return result.data;
    }

    private async Task<string> getHttp(string room)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("http://open.douyucdn.cn/api/RoomApi/room/" + room);
                if (response.EnsureSuccessStatusCode().StatusCode.ToString().ToLower() == "ok")
                {
                    string text = await response.Content.ReadAsStringAsync();
                    response.Dispose();
                    return text;
                }
            }
            catch (HttpRequestException) {
                return "";
            }
            catch (ArgumentNullException) {
                return "";
            }
        }
        return "";
    }

    public void SubscribeByUser(string user, string room)
    {
        SQLiteManager.getInstance().addSubscribe(user, room, 0);
    }

    public void SubscribeByGroup(string group, string room)
    {
        SQLiteManager.getInstance().addSubscribe(group, room, 1);
    }

    public void Desubscribe(string user, string room, int group = 0)
    {
        SQLiteManager.getInstance().deleteSubscribe(user, room, group);
    }

    private ArrayList readFromFile()
    {
        ArrayList result = new ArrayList();
        if (!File.Exists(path)) initalizeFile();

        using (StreamReader sr = new StreamReader(path))
        {
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                string temp = line.Substring(line.IndexOf('=') + 1);
                result.Add(temp);
            }
        }

        return result;
    }
}

public class DouyuDataTemp
{
    public DouyuData data;
}

public class DouyuData
{
    public int room_id;
    public int room_status;
    public string owner_name;
}