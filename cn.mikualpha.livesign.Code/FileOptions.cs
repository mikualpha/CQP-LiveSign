using System;
using System.Collections.Generic;
using System.IO;

class FileOptions
{
    private static FileOptions ins = null;
    protected string dir = null;
    protected string path = null;
    private Dictionary<string, string> fileOptions;

    private FileOptions()
    {
        dir = ApiModel.CQApi.AppDirectory;
        path = dir + "Config.ini";
        initalizeFile();
        initalizeOptions();
    }

    public static FileOptions GetInstance() {
        if (ins == null) ins = new FileOptions();
        return ins;
    }

    public Dictionary<string, string> GetOptions() { return fileOptions; }

    private void initalizeOptions()
    {
        fileOptions = new Dictionary<string, string>();
        fileOptions["Group"] = "0";
        fileOptions["Admin"] = "0";
        fileOptions["AtAll"] = "0";
        fileOptions["EnableProxy"] = "0";
        fileOptions["ProxyAddress"] = "127.0.0.1";
        fileOptions["ProxyPort"] = "1080";
        fileOptions["EasterEgg"] = "0";
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
                    "//允许哪些群成员修改群订阅设置\r\n" +
                    "Admin=123456789,987654321\r\n" +
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
}
