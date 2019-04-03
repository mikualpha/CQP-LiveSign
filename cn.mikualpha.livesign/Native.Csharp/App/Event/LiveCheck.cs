using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Native.Csharp.App;
using Native.Csharp.Tool.Http;

internal abstract class LiveCheck
{
    private Thread thread = null;
    protected readonly string dir = Common.CqApi.GetAppDirectory();
    protected readonly string path = Common.CqApi.GetAppDirectory() + "Config.ini";
    protected Dictionary<string, string> fileOptions = new Dictionary<string, string>();

    private string[] groups, admins;
    public bool running = false;
    internal enum LivingStatus { OFFLINE, ONLINE, OTHER, ERROR };

    internal LiveCheck() {
        initalizeFile();
        initalizeOptions();
    }

    private void initalizeOptions()
    {
        fileOptions.Clear();
        fileOptions["Group"] = "0";
        fileOptions["Admin"] = "0";
        fileOptions["AtAll"] = "0";
        fileOptions["EnableProxy"] = "0";
        fileOptions["ProxyAddress"] = "127.0.0.1";
        fileOptions["ProxyPort"] = "1080";
        readFromFile(path);
    }

    private bool initalizeFile()
    {
        if (File.Exists(path)) return false;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        StreamWriter writer = new StreamWriter(fs);
        writer.Write("//请仅修改等号后部分，其余部分修改可能会出现问题！\r\n" +
                    "//需要在哪些群中启用，以半角逗号分隔，标0为全部启用\r\n" +
                    "Group=0\r\n" +
                    "//允许哪些群成员修改群订阅设置，标0为全部可更改\r\n" +
                    "Admin=0\r\n" +
                    "//在群组中提醒时是否需要@全体成员，0为禁用，1为启用\r\n" + 
                    "AtAll=0\r\n" +
                    "//是否对部分平台启用代理，0为禁用，1为启用\r\n" +
                    "EnableProxy=0\r\n" + 
                    "ProxyAddress=127.0.0.1\r\n" + 
                    "ProxyPort=1080");
        writer.Close();
        fs.Close();
        return true;
    }

    public bool isGroup(string input)
    {
        groups = (fileOptions["Group"] as string).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in groups)
        {
            if (i == "0") return true;
            if (i == input) return true;
        }
        return false;
    }

    public bool isAdmin(string input)
    {
        admins = (fileOptions["Admin"] as string).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in admins)
        {
            if (i == "0") return true;
            if (i == input) return true;
        }
        return false;
    }

    public void startCheck()
    {
        if (running) return;
        running = true;
        thread = new Thread(checkStatus);
        thread.Start();
    }

    public void endCheck()
    {
        if (!running) return;
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
            string[] rooms = getSQLiteManager().getRooms();
            foreach (string i in rooms)
            {

                int room_status = getDataRoomStatus(i);
                //Common.CqApi.AddLoger(Native.Csharp.Sdk.Cqp.Enum.LogerLevel.Debug, "LivingStatusDebug-" + i, room_status.ToString());
                if (room_status == (int)LivingStatus.ERROR) continue;

                if (getSQLiteManager().getLiveStatus(i) != room_status)
                {
                    getSQLiteManager().setLiveStatus(i, room_status);
                    if (room_status == (int)LivingStatus.ONLINE) //正在直播
                    {
                        string[] users = getSQLiteManager().getUserByRoom(i); //获取所有订阅用户并发送消息
                        foreach (string j in users) { sendPrivateMessage(j); }

                        string[] groups = getSQLiteManager().getGroupByRoom(i); //获取所有订阅群组并发送消息
                        foreach (string k in groups) { sendGroupMessage(k); }
                    }
                }
            }
            Thread.Sleep(5000);
        }
    }

    private void sendGroupMessage(string group)
    {
        string msg = getOnlineMessage();
        int atAll = int.Parse(fileOptions["AtAll"] as string);
        if (atAll > 0) msg = "[CQ:at,qq=all]" + msg;
        Common.CqApi.SendGroupMessage(long.Parse(group), msg);
    }

    private void sendPrivateMessage(string qq)
    {
        Common.CqApi.SendPrivateMessage(long.Parse(qq), getOnlineMessage());
    }

    //获取订阅列表
    public string getUserSubscribe(long user)
    {
        string str = getSQLiteManager().getUserSubscribeList(user);
        if (str == "") return "列表为空！";
        string[] array = str.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        string output = "";
        for (int i = 0; i < array.Length; ++i)
        {
            if (output != "") output += "\r\n";
            output += array[i];
        }
        return output;
    }

    private string getOnlineMessage() //对返回的消息模板进行进一步处理，为附加前后缀处理预留接口
    {
        return getOnlineMessageModel();
    }

    public void SubscribeByUser(long user, string room)
    {
        getSQLiteManager().addSubscribe(user.ToString(), room, 0);
    }

    public void SubscribeByGroup(long group, string room)
    {
        getSQLiteManager().addSubscribe(group.ToString(), room, 1);
    }

    public void Desubscribe(long user, string room, int group = 0)
    {
        getSQLiteManager().deleteSubscribe(user.ToString(), room, group);
    }

    protected void readFromFile(string _path)
    {
        if (!File.Exists(_path)) initalizeFile();

        using (StreamReader sr = new StreamReader(_path))
        {
            string line = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (line.Contains("//")) continue;
                string[] temp = line.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                fileOptions[temp[0].Trim()] = temp[1].Trim();
            }
        }
    }

    protected string getProxyAddress()
    {
        return fileOptions["ProxyAddress"];
    }

    protected int getProxyPort()
    {
        return int.Parse(fileOptions["ProxyPort"]);
    }

    protected string getHttpProxy(string url)
    {
        try
        {
            if (int.Parse(fileOptions["EnableProxy"]) > 0)
            {
                WebHeaderCollection webHeaderCollection = new WebHeaderCollection();
                CookieCollection cookies = new CookieCollection();
                return Encoding.UTF8.GetString(HttpWebClient.Get(url, "", ref cookies, ref webHeaderCollection, new WebProxy(getProxyAddress(), getProxyPort()), Encoding.UTF8));
            }
            else
            {
                return Encoding.UTF8.GetString(HttpWebClient.Get(url));
            }
        } catch (WebException) { return ""; }
        
    }

    protected abstract SQLiteManager getSQLiteManager(); //获取SQLite管理实例

    protected abstract int getDataRoomStatus(string room); //检查开播状态

    public abstract string getOwnerName(string room); //启用&禁用时获取主播名

    protected abstract string getHttp(string room); //API内容获取(处理另行实现)

    protected abstract string getOnlineMessageModel(); //获取发送消息格式

    protected class FileOption //配置项
    {

    }
}
