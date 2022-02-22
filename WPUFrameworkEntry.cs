using ABHelper;
using Netwrok;
using System;
using UnityEngine;

public class WPUFrameworkEntry : MonoBehaviour
{
    //������Դ
    public static void UpdateAsset(string ip, Action<float> processChangeCB = null)
    {
        ABUpdate.Start("10.21.249.136", processChangeCB);
    }

    //����Socket
    public static void ConnectSocket()
    {
        NetworkManager.Connect("Main", "127.0.0.1", 8999, false);
        
    }
}
