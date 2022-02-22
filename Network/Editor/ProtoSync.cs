using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using UnityEditor;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class ProtoSync
{
    [MenuItem("Tools/生成协议Map代码")]
    static void SyncCode()
    {
        GenertateType();
        Debug.Log("Generate protocol code done");
        AssetDatabase.Refresh();
    }

    private static void GenertateType()
    {
        string path = Application.dataPath + "../../../ServerClientCommon/protobuf/Proto/ProtoType.proto";
        string []lines = File.ReadAllLines(path);
        string lineText = @"{0} MSGTYPE.{1}, typeof({2}) {3},//{4}";
        string types = "";
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains("Mt_S2C_"))
            {
                string temp = lines[i];
                temp = temp.Replace("Mt_S2C_", "");
                temp = temp.Replace("\t","");
                temp = temp.Replace(" ", "");
                string []splits = temp.Split('=');
                string[] splits1 = splits[1].Split(';');
                string name = splits[0];//_setTextTransform();
                types += string.Format(lineText, "{", "MtS2C" + name, "S2C" + name, "}", splits1[1]) + '\n' + '\t' + '\t' + '\t' + '\t' + '\t';
            }
        }

        string text = @"using System;
using System.Collections.Generic;
using Protocol;

/// <summary>
/// 所有需要发送和接收的协议都要在这个注册
/// </summary>
public static class MessageTypeMap
{
    static public Dictionary<MSGTYPE, Type> messageTypeDic = new Dictionary<MSGTYPE, Type>()
    {" + '\n' + '\t' + '\t' + '\t' + '\t'+ '\t' 
                    + types + '\n' +
@"  };
}";
        File.WriteAllText(Application.dataPath + @"/Scripts/PB/MessageTypeMap.cs", text);
    }

    /// <summary>
    /// 将对应的字符串转化为
    /// 对应的驼峰命名法
    /// </summary>
    /// <param name="str">以下划线分隔的字符串</param>
    /// <returns></returns>
    private static string _setTextTransform(string str)
    {
        str = str.ToLower();
        string[] temp = str.Split('_');
        string result = string.Empty;
        for (int i = 0; i < temp.Length; i++)
        {
            result += temp[i].Substring(0, 1).ToUpper() + temp[i].Substring(1);
        }

        return result;
    }


    private static void RunCommand(string workDir, string cmd)
    {
        Process process = new Process();

        var psi = process.StartInfo;
        psi.WorkingDirectory = workDir;
        psi.FileName = cmd;
        psi.CreateNoWindow = false;
        psi.ErrorDialog = true;
        psi.UseShellExecute = true;
        process.Start();
        //process.StandardInput.WriteLine(@"explorer.exe D:\");
        //process.StandardInput.WriteLine("pause");
 
        process.WaitForExit();
        process.Close();
    }
 
    private static void RunProcessCommand(string workDir, string cmd, string args)
    {
        ProcessStartInfo start = new ProcessStartInfo();
        start.WorkingDirectory = workDir;
        start.FileName = cmd;
        start.Arguments = args;
 
        start.CreateNoWindow = false;
        start.ErrorDialog = true;
        start.UseShellExecute = false;
 
        Process p = Process.Start(start);
        p.WaitForExit();
        p.Close();
    }


}
