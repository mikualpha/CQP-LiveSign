using System;
using System.Collections;
using System.IO;
using System.Threading;
using Native.Csharp.App;

internal abstract class LiveCheck
{
    private Thread thread = null;
    protected readonly string dir = Common.CqApi.GetAppDirectory();
    protected readonly string path = Common.CqApi.GetAppDirectory() + "Config.ini";
    private string[] groups, admins;
    public bool running = false;
    internal enum LivingStatus { OFFLINE, ONLINE, OTHER, ERROR };

    internal LiveCheck() { initalizeFile(); }

    private bool initalizeFile()
    {
        if (File.Exists(path)) return false;
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

        FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
        StreamWriter writer = new StreamWriter(fs);
        writer.Write("Group=0\r\n" +
                    "Admin=0\r\n" + 
                    "AtAll=0\r\n");
        writer.Close();
        fs.Close();
        return true;
    }

    public bool isGroup(string input)
    {
        groups = (readFromFile(path)[0] as string).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (string i in groups)
        {
            if (i == "0") return true;
            if (i == input) return true;
        }
        return false;
    }

    public bool isAdmin(string input)
    {
        admins = (readFromFile(path)[1] as string).Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
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
                
                if (room_status != (int)LivingStatus.ERROR && getSQLiteManager().getLiveStatus(i) != room_status)
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
        int atAll = int.Parse(readFromFile(path)[2] as string);
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

    private ArrayList readFromFile(string _path)
    {
        ArrayList result = new ArrayList();
        if (!File.Exists(_path)) initalizeFile();

        using (StreamReader sr = new StreamReader(_path))
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

    protected abstract SQLiteManager getSQLiteManager(); //获取SQLite管理实例

    protected abstract int getDataRoomStatus(string room); //检查开播状态

    public abstract string getOwnerName(string room); //启用&禁用时获取主播名

    protected abstract string getHttp(string room); //API内容获取(处理另行实现)

    protected abstract string getOnlineMessageModel(); //获取发送消息格式

}
