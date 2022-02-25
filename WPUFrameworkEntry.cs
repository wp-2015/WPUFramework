using ABHelper;
using ILRuntime.Runtime.Enviorment;
using Netwrok;
using UnityEngine;

namespace WPUFramework
{
    public class WPUFrameworkEntry : MonoBehaviour
    {
        private static System.Action<AppDomain> loadHotfixDelegate;
        public static void Entry(System.Action<AppDomain> loadHotfixDelegate)
        {
            loadHotfixDelegate = loadHotfixDelegate;
            var instance = FindObjectOfType<WPUFrameworkEntry>();
            if (instance == null)
            {
                var go = new GameObject("WPUFrameworkEntry");
                go.AddComponent<WPUFrameworkEntry>();
                DontDestroyOnLoad(go);
            }

        }

        private void Awake()
        {
            UpdateAsset("");
            ConnectSocket();
            HotFixDllEntry.Init(loadHotfixDelegate);
            CoroutineManager.Init(this);
        }

        private void Start()
        {
            HotFixDllEntry.Start();
        }

        public void Update()
        {
            NetworkManager.Update();
            HotFixDllEntry.Update();
        }

        private void FixedUpdate()
        {
            HotFixDllEntry.FixedUpdate();
        }

        private void LateUpdate()
        {
            HotFixDllEntry.LateUpdate();
        }

        //更新资源
        public static void UpdateAsset(string ip, System.Action<float> processChangeCB = null)
        {
            ABUpdate.Start("10.21.249.136", false, processChangeCB);
        }

        //链接Socket
        public static void ConnectSocket()
        {
            NetworkManager.Connect("Main", "127.0.0.1", 8999, false, ProxyInit);
        }

        public static void ProxyInit()
        {

        }
    }
}