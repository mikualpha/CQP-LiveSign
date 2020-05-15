using System;
using System.Collections.Generic;
using SQLite;

internal abstract class SQLiteManager
{
    internal enum LivingStatus { OFFLINE, ONLINE, OTHER };
    private SQLiteConnection _connection = null;

    internal SQLiteManager()
    {
        _connection = new SQLiteConnection(getDatabasePath());
        createTable();
    }

    public abstract string getDatabasePath();

    //创建表
    private void createTable()
    {
        _connection.CreateTable<SubScribeUser>();
        _connection.CreateTable<SubScribeStatus>();
    }

    //添加订阅
    public void addSubscribe(int user, int room, int group = 0) { addSubscribe(user.ToString(), room.ToString(), group); }

    public void addSubscribe(string user, string room, int group = 0)
    {
        addRoom(room);

        string[] sub_rooms = getSubscribeByUser(user);
        string output = "";

        if (sub_rooms != null)
        {
            foreach (string i in sub_rooms)
            {
                if (i == room) return;
                if (output != "") output += ",";
                output += i;
            }
        }

        if (output != "") output += ",";
        output += room;

        insertSubscribe(user, output, group);
    }

    //删除订阅
    public void deleteSubscribe(string user, string room, int group = 0)
    {
        string[] sub_rooms = getSubscribeByUser(user);
        string output = "";

        if (sub_rooms != null)
        {
            foreach (string i in sub_rooms)
            {
                if (i == room) continue;
                if (output != "") output += ",";
                output += i;
            }
        }

        if (output == null || output == "") deleteUser(user); //若该用户订阅数为空，则删除该用户
        else insertSubscribe(user, output, group); //否则更改用户订阅信息

        if (getUserByRoom(room).Length == 0 && getGroupByRoom(room).Length == 0) deleteRoom(room); //若该房间已无用户/群组订阅，则删除此房间
    }

    //插入订阅记录(若已有记录则变更)
    private void insertSubscribe(string _user, string rooms, int group = 0)
    {
        _connection.Insert(new SubScribeUser()
        {
            user = _user,
            sub_rooms = rooms,
            is_group = group
        }, "OR REPLACE");
    }

    //删除用户
    private void deleteUser(string _user)
    {
        _connection.Delete(new SubScribeUser() { user = _user });
    }

    //添加房间(若有则无视)
    private void addRoom(int room) { addRoom(room.ToString()); }

    private void addRoom(string _room)
    {
        _connection.Insert(new SubScribeStatus()
        {
            room = _room,
            living = (int)LivingStatus.OFFLINE
        }, "OR IGNORE");

    }

    //删除房间(若不存在则无视)
    private void deleteRoom(int room) { deleteRoom(room.ToString()); }

    private void deleteRoom(string _room)
    {
        _connection.Delete(new SubScribeStatus() { room = _room });
    }

    //检查某用户是否已订阅某房间
    public bool isSubscribing(string user, string room)
    {
        string[] temp = getSubscribeByUser(user);
        foreach (string i in temp)
        {
            if (i == room) return true;
        }
        return false;
    }

    //获取用户订阅的所有房间
    public string[] getSubscribeByUser(string user)
    {
        List<SubScribeUser> temp = _connection.Query<SubScribeUser>("SELECT sub_rooms FROM SubScribeUser WHERE user = ?", user);
        if (temp.Count <= 0) return null;
        return temp[0].sub_rooms.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    //根据房间获取订阅用户表
    public string[] getUserByRoom(string room)
    {
        return getSubScriberList(room, 0);
    }

    //根据房间获取订阅群组表
    public string[] getGroupByRoom(string room)
    {
        return getSubScriberList(room, 1);
    }

    //获取订阅列表
    private string[] getSubScriberList(string room, int isGroup)
    {
        List<SubScribeUser> temp = _connection.Query<SubScribeUser>(
            "SELECT user FROM SubScribeUser WHERE is_group = ? AND (" +
            "sub_rooms LIKE '%," + room + ",%' " +
            "OR sub_rooms LIKE '" + room + ",%' " +
            "OR sub_rooms LIKE '%," + room + "' " +
            "OR sub_rooms = '" + room + "')", isGroup);
        string output = "";
        for (int i = 0; i < temp.Count; ++i)
        {
            if (output != "") output += ",";
            output += temp[i].user;
        }
        return output.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    //获取所有的房间列表
    public string[] getRooms()
    {
        List<SubScribeStatus> temp = _connection.Query<SubScribeStatus>("SELECT room FROM SubScribeStatus");
        string output = "";
        for (int i = 0; i < temp.Count; ++i)
        {
            if (output != "") output += ",";
            output += temp[i].room.ToString();
        }
        return output.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
    }

    //获取群组的订阅列表
    public string getUserSubscribeList(long user)
    {
        List<SubScribeUser> temp = _connection.Query<SubScribeUser>("SELECT sub_rooms FROM SubScribeUser WHERE user = ?", user);
        if (temp.Count <= 0) return "";
        return temp[0].sub_rooms;
    }

    //设置Live状态
    public void setLiveStatus(string _room, int status)
    {
        _connection.Update(new SubScribeStatus()
        {
            room = _room,
            living = status
        });
    }

    //获取Live状态
    public int getLiveStatus(string room)
    {
        List<SubScribeStatus> temp = _connection.Query<SubScribeStatus>("SELECT living FROM SubScribeStatus WHERE room = ?", room);
        if (temp.Count <= 0) return (int)LivingStatus.OTHER;
        return temp[0].living;
    }

    protected class SubScribeUser
    {
        [PrimaryKey, NotNull]
        public string user { get; set; }
        public string sub_rooms { get; set; }
        [NotNull]
        public int is_group { get; set; }
    }

    protected class SubScribeStatus
    {
        [PrimaryKey]
        public string room { get; set; }
        [NotNull]
        public int living { get; set; }
    }
}

